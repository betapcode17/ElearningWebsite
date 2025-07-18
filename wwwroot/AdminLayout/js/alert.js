setTimeout(() => {
  const success = document.getElementById("customAlertSuccess");
  if (success) {
    success.style.opacity = "0";
    success.style.transform = "translateY(-10px)";
    setTimeout(() => success.remove(), 500);
  }

  const error = document.getElementById("customAlertError");
  if (error) {
    error.style.opacity = "0";
    error.style.transform = "translateY(-10px)";
    setTimeout(() => error.remove(), 500);
  }
}, 4000);
