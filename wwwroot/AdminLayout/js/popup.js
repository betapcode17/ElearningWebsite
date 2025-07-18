function openPopup(courseId) {
  document.getElementById("popup-overlay").style.display = "flex";
  document.getElementById("popup-course-id").value = courseId;
}

function closePopup() {
  document.getElementById("popup-overlay").style.display = "none";
}
