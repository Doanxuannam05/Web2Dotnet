---
name: web-motion-design
description: >-
  Chuyên gia thiết kế hiệu ứng chuyển động (Motion Design), tương tác micro-interactions, cuộn mượt (smooth scroll), chuyển cảnh trang (page transitions), và hiệu ứng đồ họa phong cách Awwwards/FWA cho website. Kích hoạt khi người dùng muốn thêm animation, tương tác chuyển động, cấu hình GSAP, Framer Motion, Lenis Scroll, Three.js hoặc CSS Scroll-Driven Animations.
---

# Web Motion & Creative Animation Design Mastery

Tài liệu hướng dẫn toàn diện về thiết kế và lập trình hiệu ứng chuyển động (Motion UI / Web Animations) chuẩn hiện đại, mượt mà (60–120 FPS), tối ưu hiệu năng và đạt tính thẩm mỹ cao.

---

## 1. Triết lý Motion Design (Core Principles)

1. **Functional over Decorative**: Hiệu ứng phải dẫn dắt mắt người dùng, cung cấp phản hồi thị giác (visual feedback) và làm rõ cấu trúc không gian, tránh lạm dụng gây rối mắt hoặc làm chậm thao tác.
2. **Spring Physics & Natural Easing**:
   - Ưu tiên Spring Physics (lò xo có trọng lượng, lực đàn hồi) thay cho các chuyển động tuyến tính (linear) cứng nhắc.
   - Đường cong Easing cao cấp (Cinematic Out): `cubic-bezier(0.16, 1, 0.3, 1)` (Quart/Expo Out) mang lại cảm giác phản hồi tức thì nhưng dừng lại êm ái.
3. **Thời lượng chuẩn (Timing)**:
   - **Micro-interactions** (hover, click, toggle, tooltip): `150ms – 250ms`.
   - **Thành phần UI cỡ trung** (dialog, modal, drawer, dropdown): `250ms – 400ms`.
   - **Chuyển cảnh lớn / Page Transitions / Hero reveals**: `400ms – 800ms`.
4. **Accessibility (Bắt buộc)**:
   - Luôn tôn trọng người dùng bằng cách kiểm tra `@media (prefers-reduced-motion: reduce)`. Tắt hoặc giảm biên độ hiệu ứng về mức tối thiểu khi người dùng có thiết lập này.

---

## 2. Quy tắc Vàng về Hiệu Năng (60–120 FPS)

* **Chỉ animate các thuộc tính Composting (GPU-accelerated)**:
  - `transform` (`translateX`, `translateY`, `translateZ`, `scale`, `rotate`)
  - `opacity`
  - `clip-path` hoặc `filter` (dùng có kiểm soát)
* **TUYỆT ĐỐI KHÔNG animate**: `top`, `left`, `bottom`, `right`, `width`, `height`, `margin`, `padding` vì sẽ kích hoạt **Browser Layout & Reflow** gây giật lag (jank/dropped frames).
* **Tối ưu Layer**:
  - Dùng `will-change: transform, opacity` cho các phần tử chuyển động liên tục, nhưng chỉ áp dụng khi bắt đầu animate và gỡ bỏ khi kết thúc để tránh tốn RAM đồ họa.

---

## 3. Các Tech Stack & Thư viện Hàng đầu

### A. Framer Motion / Motion (Tốt nhất cho React & Next.js)
Dành cho UI động, spring physics, layout morphing:

```tsx
import { motion, AnimatePresence } from "framer-motion";

// 1. Stagger Container & Child Reveal
const containerVariants = {
  hidden: { opacity: 0 },
  show: {
    opacity: 1,
    transition: { staggerChildren: 0.08, delayChildren: 0.1 }
  }
};

const itemVariants = {
  hidden: { opacity: 0, y: 24, filter: "blur(4px)" },
  show: {
    opacity: 1,
    y: 0,
    filter: "blur(0px)",
    transition: { type: "spring", stiffness: 280, damping: 24 }
  }
};

// 2. Shared Layout Transition (morph thẻ/tab)
// Dùng prop `layoutId="unique-id"` trên cả 2 element để biến hình mượt mà.
```

### B. GSAP (GreenSock) + ScrollTrigger (Tốt nhất cho Creative Websites / Awwwards)
Dành cho timeline phức tạp, ghim cuộn (pinning), cuộn theo tiến độ (scrubbing):

```javascript
import gsap from "gsap";
import { ScrollTrigger } from "gsap/ScrollTrigger";
gsap.registerPlugin(ScrollTrigger);

// Timeline cuộn và ghim phần tử (Pin & Scrub)
const tl = gsap.timeline({
  scrollTrigger: {
    trigger: ".hero-section",
    start: "top top",
    end: "+=150%",
    pin: true,
    scrub: 1, // Cuộn mượt theo tay người dùng với độ trễ 1s
    anticipatePin: 1
  }
});

tl.from(".hero-title", { y: 100, opacity: 0, ease: "power3.out" })
  .from(".hero-image", { scale: 1.3, filter: "blur(8px)", ease: "power2.out" }, "<0.2");
```

> **Lưu ý trong React/Next.js**: Luôn gói code GSAP trong `useGSAP()` (từ `@gsap/react`) để tự động dọn dẹp (cleanup/revert) khi unmount, chống rò rỉ bộ nhớ (memory leak).

### C. Lenis (Smooth Scroll chuẩn hiện đại)
Đem lại trải nghiệm cuộn mượt mà như bơ (butter-smooth) kết hợp hoàn hảo với GSAP:

```javascript
import Lenis from "lenis";

const lenis = new Lenis({
  duration: 1.2,
  easing: (t) => Math.min(1, 1.001 - Math.pow(2, -10 * t)),
  smoothWheel: true
});

function raf(time) {
  lenis.raf(time);
  requestAnimationFrame(raf);
}
requestAnimationFrame(raf);
```

### D. Modern Native Web Animations (Không cần thư viện)
1. **Scroll-Driven Animations (CSS thuần)**:
   ```css
   @keyframes revealOnScroll {
     from { opacity: 0; transform: translateY(40px); }
     to { opacity: 1; transform: translateY(0); }
   }

   .card {
     animation: revealOnScroll linear both;
     animation-timeline: view();
     animation-range: entry 20% cover 40%;
   }
   ```
2. **View Transitions API (Chuyển trang SPA/MPA mượt mà)**:
   ```javascript
   if (document.startViewTransition) {
     document.startViewTransition(() => {
       updateDOM();
     });
   } else {
     updateDOM();
   }
   ```

---

## 4. Công thức Thiết kế các Hiệu ứng Thường Gặp (Design Recipes)

1. **Text Split & Stagger Reveal**: Chia tách từ/ký tự (`<span>`), xuất hiện lần lượt từ dưới lên với `y: "100%" -> 0%` kèm `overflow: hidden` ở thẻ bao bọc (mask reveal).
2. **Magnetic Cursor / Button**: Nút bấm bị hút nhẹ theo toạ độ trỏ chuột khi hover (tính toán offset `(clientX - rect.left) / rect.width`).
3. **Parallax Image Container**: Tấm ảnh bên trong thẻ `overflow: hidden` có chiều cao `120%`, translate `y: -10% -> 10%` theo tiến độ cuộn.
4. **Infinite Marquee**: Dòng chữ hoặc logo chạy vô tận dùng CSS `transform: translateX(-50%)` lặp lại tuần hoàn với tốc độ ổn định, tạm dừng khi hover.
