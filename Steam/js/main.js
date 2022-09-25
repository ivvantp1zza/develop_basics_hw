const menu_btn = document.querySelector("#menu_btn");
const menu = document.querySelector("#adaptive_menu");
const cancel_menu_btn = document.querySelector("#center_cont");
menu_btn.addEventListener("click", () => {menu.classList.toggle("menu_active")});
cancel_menu_btn.addEventListener("click", () => {menu.classList.remove("menu_active")});

