🎓 Training Center Management System
📌 Introduction
This is a Training Center Management System built using ASP.NET MVC, allowing administrators to manage courses, students, and course enrollment processes. The application supports both Students and Admins, and is suitable for training centers, small educational institutions, or internal learning systems.

🚀 Main Features
1. Course Management
Course Information: Course ID, Course Name, Instructor, Start Date, Tuition Fee, Maximum Number of Students.

Functions:

Add / Edit / Delete courses.

Display list of available courses.

2. Student Management
Student Information: Student ID, Full Name, Date of Birth, Phone Number, Email, Username, Password.

Functions:

Students can register an account and log in to the system.

3. Course Enrollment
Students can choose and enroll in available courses.

The system checks whether the course has reached the maximum number of students before allowing registration.

Students can cancel enrollment before the course start date.

🛠 Technical Requirements
Use ASP.NET MVC to develop the application.

Use Entity Framework for database operations.

Design UI using Bootstrap or plain CSS.

✅ Basic Requirements
Set up and configure a standard ASP.NET MVC project.

Create appropriate Models for database interaction.

Implement CRUD operations (Create, Read, Update, Delete) for courses and students.

Develop course enrollment features with constraints.

Check course capacity before confirming student registration.

👥 User Roles
The system has 2 main roles:

Student

Admin (full access to manage data, track courses, generate reports, etc.)

