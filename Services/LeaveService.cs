using DemoAPINET.DTOs;
using DemoAPINET.DTOs.Leave;
using DemoAPINET.DTOs.LeaveType;
using DemoAPINET.Models;
using DemoAPINET.Repositories;

namespace DemoAPINET.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public LeaveService(
            ILeaveRequestRepository leaveRequestRepository,
            ILeaveTypeRepository leaveTypeRepository,
            IEmployeeRepository employeeRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _employeeRepository = employeeRepository;
        }

        // ==========================================
        // 1. Quản lý Danh mục Loại nghỉ phép (LeaveType)
        // ==========================================

        public async Task<List<LeaveTypeResponseDto>> GetAllLeaveTypesAsync()
        {
            var types = await _leaveTypeRepository.GetAllAsync();
            return types.Select(lt => new LeaveTypeResponseDto
            {
                Id = lt.Id,
                Name = lt.Name,
                DefaultDays = lt.DefaultDays,
                IsPaid = lt.IsPaid,
                Description = lt.Description
            }).ToList();
        }

        public async Task<LeaveTypeResponseDto?> GetLeaveTypeByIdAsync(int id)
        {
            var lt = await _leaveTypeRepository.GetByIdAsync(id);
            if (lt == null) return null;

            return new LeaveTypeResponseDto
            {
                Id = lt.Id,
                Name = lt.Name,
                DefaultDays = lt.DefaultDays,
                IsPaid = lt.IsPaid,
                Description = lt.Description
            };
        }

        public async Task<LeaveTypeResponseDto> CreateLeaveTypeAsync(LeaveTypeCreateDto dto)
        {
            var lt = new LeaveType
            {
                Name = dto.Name.Trim(),
                DefaultDays = dto.DefaultDays,
                IsPaid = dto.IsPaid,
                Description = dto.Description?.Trim()
            };

            await _leaveTypeRepository.AddAsync(lt);
            await _leaveTypeRepository.SaveChangesAsync();

            return new LeaveTypeResponseDto
            {
                Id = lt.Id,
                Name = lt.Name,
                DefaultDays = lt.DefaultDays,
                IsPaid = lt.IsPaid,
                Description = lt.Description
            };
        }

        public async Task<bool> UpdateLeaveTypeAsync(int id, LeaveTypeUpdateDto dto)
        {
            var lt = await _leaveTypeRepository.GetByIdAsync(id);
            if (lt == null) return false;

            lt.Name = dto.Name.Trim();
            lt.DefaultDays = dto.DefaultDays;
            lt.IsPaid = dto.IsPaid;
            lt.Description = dto.Description?.Trim();

            _leaveTypeRepository.Update(lt);
            return await _leaveTypeRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteLeaveTypeAsync(int id)
        {
            var lt = await _leaveTypeRepository.GetByIdAsync(id);
            if (lt == null) return false;

            _leaveTypeRepository.Delete(lt);
            return await _leaveTypeRepository.SaveChangesAsync();
        }

        // ==========================================
        // 2. Quản lý Đơn xin nghỉ (LeaveRequest)
        // ==========================================

        public async Task<PagedResponse<LeaveRequestResponseDto>> GetPagedRequestsAsync(LeaveQueryParameters query)
        {
            var (items, totalCount) = await _leaveRequestRepository.GetPagedAsync(query);
            var dtos = items.Select(MapToResponseDto);
            return new PagedResponse<LeaveRequestResponseDto>(dtos, totalCount, query.PageNumber, query.PageSize);
        }

        public async Task<List<LeaveRequestResponseDto>> GetMyRequestsAsync(int userId)
        {
            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            if (employee == null)
            {
                return new List<LeaveRequestResponseDto>();
            }

            var requests = await _leaveRequestRepository.GetByEmployeeIdAsync(employee.Id);
            return requests.Select(MapToResponseDto).ToList();
        }

        public async Task<LeaveRequestResponseDto?> GetRequestByIdAsync(int id)
        {
            var request = await _leaveRequestRepository.GetByIdAsync(id);
            return request == null ? null : MapToResponseDto(request);
        }

        public async Task<LeaveRequestResponseDto> CreateRequestAsync(int userId, LeaveRequestCreateDto dto)
        {
            // 1. Xác định nhân viên nộp đơn
            Employee? employee;
            if (dto.EmployeeId.HasValue && dto.EmployeeId.Value > 0)
            {
                employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId.Value);
            }
            else
            {
                employee = await _employeeRepository.GetByUserIdAsync(userId);
            }

            if (employee == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hồ sơ nhân sự của tài khoản hiện tại.");
            }

            // 2. Kiểm tra loại nghỉ phép
            var leaveType = await _leaveTypeRepository.GetByIdAsync(dto.LeaveTypeId);
            if (leaveType == null)
            {
                throw new KeyNotFoundException("Loại nghỉ phép được chọn không tồn tại.");
            }

            // 3. Kiểm tra tính hợp lệ của thời gian
            if (dto.StartDate.Date > dto.EndDate.Date)
            {
                throw new ArgumentException("Ngày bắt đầu nghỉ không được lớn hơn ngày kết thúc.");
            }

            var totalDays = (dto.EndDate.Date - dto.StartDate.Date).Days + 1;

            // 4. Kiểm tra trùng lặp lịch nghỉ
            var isOverlapping = await _leaveRequestRepository.HasOverlappingRequestAsync(
                employee.Id, dto.StartDate.Date, dto.EndDate.Date);
            if (isOverlapping)
            {
                throw new InvalidOperationException("Bạn đã có đơn xin nghỉ phép khác trong khoảng thời gian này.");
            }

            // 5. Kiểm tra hạn mức ngày nghỉ phép năm còn lại
            if (leaveType.IsPaid && employee.RemainingLeaveDays < totalDays)
            {
                throw new InvalidOperationException(
                    $"Số ngày xin nghỉ ({totalDays} ngày) vượt quá số ngày phép còn lại ({employee.RemainingLeaveDays} ngày).");
            }

            // 6. Tạo đơn ở trạng thái Pending
            var request = new LeaveRequest
            {
                EmployeeId = employee.Id,
                LeaveTypeId = dto.LeaveTypeId,
                StartDate = dto.StartDate.Date,
                EndDate = dto.EndDate.Date,
                TotalDays = totalDays,
                Reason = dto.Reason.Trim(),
                Status = LeaveStatus.Pending,
                AttachedFileId = dto.AttachedFileId,
                CreatedAt = DateTime.UtcNow
            };

            await _leaveRequestRepository.AddAsync(request);
            await _leaveRequestRepository.SaveChangesAsync();

            var created = await _leaveRequestRepository.GetByIdAsync(request.Id);
            return MapToResponseDto(created ?? request);
        }

        public async Task<(bool Success, string Message)> CancelRequestAsync(int id, int userId)
        {
            var request = await _leaveRequestRepository.GetByIdAsync(id);
            if (request == null)
            {
                return (false, "Không tìm thấy đơn xin nghỉ.");
            }

            // Kiểm tra quyền sở hữu đơn
            var employee = await _employeeRepository.GetByUserIdAsync(userId);
            if (employee == null || request.EmployeeId != employee.Id)
            {
                return (false, "Bạn không có quyền hủy đơn xin nghỉ của nhân viên khác.");
            }

            if (request.Status != LeaveStatus.Pending)
            {
                return (false, "Chỉ có thể hủy đơn xin nghỉ khi đang ở trạng thái 'Chờ duyệt' (Pending).");
            }

            request.Status = LeaveStatus.Cancelled;
            request.UpdatedAt = DateTime.UtcNow;

            _leaveRequestRepository.Update(request);
            await _leaveRequestRepository.SaveChangesAsync();

            return (true, "Đã hủy đơn xin nghỉ phép thành công.");
        }

        public async Task<(bool Success, string Message)> ApproveRequestAsync(int id, int approverUserId)
        {
            var request = await _leaveRequestRepository.GetByIdAsync(id);
            if (request == null)
            {
                return (false, "Không tìm thấy đơn xin nghỉ phép.");
            }

            if (request.Status != LeaveStatus.Pending)
            {
                return (false, "Đơn này đã được xử lý trước đó, không thể phê duyệt lại.");
            }

            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                return (false, "Không tìm thấy hồ sơ nhân viên tương ứng.");
            }

            // Trừ số ngày nghỉ còn lại nếu loại nghỉ có tính lương/trừ phép
            if (request.LeaveType != null && request.LeaveType.IsPaid)
            {
                employee.RemainingLeaveDays = Math.Max(0, employee.RemainingLeaveDays - request.TotalDays);
                _employeeRepository.Update(employee);
            }

            request.Status = LeaveStatus.Approved;
            request.ApprovedByUserId = approverUserId;
            request.UpdatedAt = DateTime.UtcNow;

            _leaveRequestRepository.Update(request);
            await _leaveRequestRepository.SaveChangesAsync();

            return (true, "Phê duyệt đơn xin nghỉ phép thành công.");
        }

        public async Task<(bool Success, string Message)> RejectRequestAsync(int id, int approverUserId, string rejectionReason)
        {
            var request = await _leaveRequestRepository.GetByIdAsync(id);
            if (request == null)
            {
                return (false, "Không tìm thấy đơn xin nghỉ phép.");
            }

            if (request.Status != LeaveStatus.Pending)
            {
                return (false, "Đơn này đã được xử lý trước đó.");
            }

            request.Status = LeaveStatus.Rejected;
            request.ApprovedByUserId = approverUserId;
            request.RejectionReason = string.IsNullOrWhiteSpace(rejectionReason) 
                ? "Không đáp ứng yêu cầu công việc thời điểm hiện tại" 
                : rejectionReason.Trim();
            request.UpdatedAt = DateTime.UtcNow;

            _leaveRequestRepository.Update(request);
            await _leaveRequestRepository.SaveChangesAsync();

            return (true, "Đã từ chối đơn xin nghỉ phép.");
        }

        public async Task<LeaveStatisticsDto> GetStatisticsAsync(int? userId = null)
        {
            if (userId.HasValue)
            {
                var employee = await _employeeRepository.GetByUserIdAsync(userId.Value);
                if (employee != null)
                {
                    return await _leaveRequestRepository.GetStatisticsAsync(employee.Id);
                }
            }

            return await _leaveRequestRepository.GetStatisticsAsync(null);
        }

        private static LeaveRequestResponseDto MapToResponseDto(LeaveRequest lr)
        {
            return new LeaveRequestResponseDto
            {
                Id = lr.Id,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee?.FullName ?? string.Empty,
                DepartmentName = lr.Employee?.Department?.Name,
                LeaveTypeId = lr.LeaveTypeId,
                LeaveTypeName = lr.LeaveType?.Name ?? string.Empty,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                TotalDays = lr.TotalDays,
                Reason = lr.Reason,
                Status = lr.Status,
                RejectionReason = lr.RejectionReason,
                ApprovedByUserId = lr.ApprovedByUserId,
                ApprovedByUserName = lr.ApprovedByUser?.UserName,
                AttachedFileId = lr.AttachedFileId,
                AttachedFileName = lr.AttachedFile?.FileName,
                CreatedAt = lr.CreatedAt,
                UpdatedAt = lr.UpdatedAt
            };
        }
    }
}
