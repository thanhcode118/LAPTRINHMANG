/*
  Inspired by: "Login Page & Homepage"
  By: Neo  Link: https://dribbble.com/shots/4485321-Login-Page-Homepage
*/

let emailInput = document.querySelector(".username");  // input email
let passwordInput = document.querySelector(".password");
let showPasswordButton = document.querySelector(".password-button");
let face = document.querySelector(".face");

// Khi focus vào input password => tay che mặt, lưỡi ngừng thở
passwordInput.addEventListener("focus", () => {
    document.querySelectorAll(".hand").forEach(hand => hand.classList.add("hide"));
    document.querySelector(".tongue").classList.remove("breath");
});

// Khi blur khỏi input password => tay trở lại, lưỡi thở lại
passwordInput.addEventListener("blur", () => {
    document.querySelectorAll(".hand").forEach(hand => {
        hand.classList.remove("hide");
        hand.classList.remove("peek");
    });
    document.querySelector(".tongue").classList.add("breath");
});

// Khi focus vào input email => mặt quay theo số ký tự nhập
emailInput.addEventListener("focus", () => {
    // Giới hạn góc quay tối đa 19 độ, tính dựa vào độ dài email trừ 16
    let length = Math.min(emailInput.value.length - 16, 19);
    document.querySelectorAll(".hand").forEach(hand => {
        hand.classList.remove("hide");
        hand.classList.remove("peek");
    });
    face.style.setProperty("--rotate-head", `${-length}deg`);
});

// Khi blur khỏi input email => mặt trở về góc 0
emailInput.addEventListener("blur", () => {
    face.style.setProperty("--rotate-head", "0deg");
});

// Khi nhập input email => mặt quay theo số ký tự nhập (throttled để giảm số lần gọi)
emailInput.addEventListener("input", _.throttle(event => {
    let length = Math.min(event.target.value.length - 16, 19);
    face.style.setProperty("--rotate-head", `${-length}deg`);
}, 100));

// Xử lý nút show/hide mật khẩu
showPasswordButton.addEventListener("click", (event) => {
    event.preventDefault(); // Ngăn form submit nếu có

    if (passwordInput.type === "text") {
        passwordInput.type = "password";
        document.querySelectorAll(".hand").forEach(hand => {
            hand.classList.remove("peek");
            hand.classList.add("hide");
        });
        showPasswordButton.textContent = "show";
    } else {
        passwordInput.type = "text";
        document.querySelectorAll(".hand").forEach(hand => {
            hand.classList.remove("hide");
            hand.classList.add("peek");
        });
        showPasswordButton.textContent = "hide";
    }
});
