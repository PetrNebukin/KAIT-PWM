using Microsoft.Data.Sqlite;
using PracticalWorksManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Storage;

namespace PracticalWorksManager.Services
{
    public class DatabaseService
    {
        private const string DatabaseFileName = "PracticalWorks.db";
        private readonly string _databasePath;

        public DatabaseService()
        {
            _databasePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DatabaseFileName);
        }

        public void InitializeDatabase()
        {
            CreateDatabase();
            CreateTables();
        }

        private async void CreateDatabase()
        {
            if (!File.Exists(_databasePath))
            {
                StorageFile databaseFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(DatabaseFileName);
            }
        }

        private void CreateTables()
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                // Create Groups table
                var createGroupsTableCommand = new SqliteCommand(@"
                    CREATE TABLE IF NOT EXISTS Groups (
                        GroupID INTEGER PRIMARY KEY AUTOINCREMENT,
                        GroupName TEXT NOT NULL
                    );
                ", db);
                createGroupsTableCommand.ExecuteNonQuery();

                // Create Students table
                var createStudentsTableCommand = new SqliteCommand(@"
                    CREATE TABLE IF NOT EXISTS Students (
                        StudentID INTEGER PRIMARY KEY AUTOINCREMENT,
                        GroupID INTEGER NOT NULL,
                        FirstName TEXT NOT NULL,
                        LastName TEXT NOT NULL,
                        FOREIGN KEY(GroupID) REFERENCES Groups(GroupID)
                    );
                ", db);
                createStudentsTableCommand.ExecuteNonQuery();

                // Create Assignments table
                var createAssignmentsTableCommand = new SqliteCommand(@"
                    CREATE TABLE IF NOT EXISTS Assignments (
                        AssignmentID INTEGER PRIMARY KEY AUTOINCREMENT,
                        AssignmentName TEXT NOT NULL,
                        Description TEXT
                    );
                ", db);
                createAssignmentsTableCommand.ExecuteNonQuery();

                // Create Deadlines table
                var createDeadlinesTableCommand = new SqliteCommand(@"
                    CREATE TABLE IF NOT EXISTS Deadlines (
                        DeadlineID INTEGER PRIMARY KEY AUTOINCREMENT,
                        StudentID INTEGER NOT NULL,
                        AssignmentID INTEGER NOT NULL,
                        DeadlineDate TEXT NOT NULL,
                        SubmissionDate TEXT,
                        FOREIGN KEY(StudentID) REFERENCES Students(StudentID),
                        FOREIGN KEY(AssignmentID) REFERENCES Assignments(AssignmentID)
                    );
                ", db);
                createDeadlinesTableCommand.ExecuteNonQuery();
            }
        }

        // GROUPS

        public void AddGroup(Group group)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var insertCommand = new SqliteCommand(@"
                    INSERT INTO Groups (GroupName)
                    VALUES (@GroupName);
                ", db);
                insertCommand.Parameters.AddWithValue("@GroupName", group.GroupName);
                insertCommand.ExecuteNonQuery();
            }
        }

        public ObservableCollection<Group> GetGroups()
        {
            ObservableCollection<Group> groups = new ObservableCollection<Group>();

            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand(@"
                    SELECT GroupID, GroupName FROM Groups;
                ", db);

                using (SqliteDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Group group = new Group();
                        group.GroupID = reader.GetInt32(0);
                        group.GroupName = reader.GetString(1);
                        groups.Add(group);
                    }
                }
            }

            return groups;
        }

        public void UpdateGroup(Group group)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var updateCommand = new SqliteCommand(@"
                    UPDATE Groups
                    SET GroupName = @GroupName
                    WHERE GroupID = @GroupID;
                ", db);
                updateCommand.Parameters.AddWithValue("@GroupName", group.GroupName);
                updateCommand.Parameters.AddWithValue("@GroupID", group.GroupID);
                updateCommand.ExecuteNonQuery();
            }
        }

        public void DeleteGroup(int groupId)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var deleteCommand = new SqliteCommand(@"
                    DELETE FROM Groups
                    WHERE GroupID = @GroupID;
                ", db);
                deleteCommand.Parameters.AddWithValue("@GroupID", groupId);
                deleteCommand.ExecuteNonQuery();
            }
        }

        public int GetLastGroupId()
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand(@"SELECT last_insert_rowid();", db);
                return Convert.ToInt32(selectCommand.ExecuteScalar());
            }
        }

        // STUDENTS

        public ObservableCollection<Student> GetAllStudents()
        {
            ObservableCollection<Student> students = new ObservableCollection<Student>();

            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand(@"
                    SELECT StudentID, GroupID, FirstName, LastName
                    FROM Students;
                ", db);

                using (SqliteDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Student student = new Student();
                        student.StudentID = reader.GetInt32(0);
                        student.GroupID = reader.GetInt32(1);
                        student.FirstName = reader.GetString(2);
                        student.LastName = reader.GetString(3);
                        students.Add(student);
                    }
                }
            }

            return students;
        }
        public void AddStudent(Student student)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var insertCommand = new SqliteCommand(@"
                    INSERT INTO Students (GroupID, FirstName, LastName)
                    VALUES (@GroupID, @FirstName, @LastName);
                ", db);
                insertCommand.Parameters.AddWithValue("@GroupID", student.GroupID);
                insertCommand.Parameters.AddWithValue("@FirstName", student.FirstName);
                insertCommand.Parameters.AddWithValue("@LastName", student.LastName);
                insertCommand.ExecuteNonQuery();
            }
        }

        public ObservableCollection<Student> GetStudents(int groupId)
        {
            ObservableCollection<Student> students = new ObservableCollection<Student>();

            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand(@"
                    SELECT StudentID, GroupID, FirstName, LastName
                    FROM Students
                    WHERE GroupID = @GroupID;
                ", db);
                selectCommand.Parameters.AddWithValue("@GroupID", groupId);

                using (SqliteDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Student student = new Student();
                        student.StudentID = reader.GetInt32(0);
                        student.GroupID = reader.GetInt32(1);
                        student.FirstName = reader.GetString(2);
                        student.LastName = reader.GetString(3);
                        students.Add(student);
                    }
                }
            }

            return students;
        }

        public void UpdateStudent(Student student)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var updateCommand = new SqliteCommand(@"
                    UPDATE Students
                    SET GroupID = @GroupID,
                        FirstName = @FirstName,
                        LastName = @LastName
                    WHERE StudentID = @StudentID;
                ", db);
                updateCommand.Parameters.AddWithValue("@GroupID", student.GroupID);
                updateCommand.Parameters.AddWithValue("@FirstName", student.FirstName);
                updateCommand.Parameters.AddWithValue("@LastName", student.LastName);
                updateCommand.Parameters.AddWithValue("@StudentID", student.StudentID);
                updateCommand.ExecuteNonQuery();
            }
        }

        public void DeleteStudent(int studentId)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var deleteCommand = new SqliteCommand(@"
                    DELETE FROM Students
                    WHERE StudentID = @StudentID;
                ", db);
                deleteCommand.Parameters.AddWithValue("@StudentID", studentId);
                deleteCommand.ExecuteNonQuery();
            }
        }

        public int GetLastStudentId()
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand(@"SELECT last_insert_rowid();", db);
                return Convert.ToInt32(selectCommand.ExecuteScalar());
            }
        }

        // ASSIGNMENTS

        public int GetLastAssignmentId()
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand(@"SELECT last_insert_rowid();", db);
                return Convert.ToInt32(selectCommand.ExecuteScalar());
            }
        }

        public void AddAssignment(Assignment assignment)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var insertCommand = new SqliteCommand(@"
                    INSERT INTO Assignments (AssignmentName, Description)
                    VALUES (@AssignmentName, @Description);
                ", db);
                insertCommand.Parameters.AddWithValue("@AssignmentName", assignment.AssignmentName);
                insertCommand.Parameters.AddWithValue("@Description", assignment.Description);
                insertCommand.ExecuteNonQuery();
            }
        }

        public ObservableCollection<Assignment> GetAssignments()
        {
            ObservableCollection<Assignment> assignments = new ObservableCollection<Assignment>();

            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand(@"
                    SELECT AssignmentID, AssignmentName, Description FROM Assignments;
                ", db);

                using (SqliteDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Assignment assignment = new Assignment();
                        assignment.AssignmentID = reader.GetInt32(0);
                        assignment.AssignmentName = reader.GetString(1);
                        assignment.Description = reader.GetString(2);
                        assignments.Add(assignment);
                    }
                }
            }

            return assignments;
        }

        public void UpdateAssignment(Assignment assignment)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var updateCommand = new SqliteCommand(@"
                    UPDATE Assignments
                    SET AssignmentName = @AssignmentName,
                        Description = @Description
                    WHERE AssignmentID = @AssignmentID;
                ", db);
                updateCommand.Parameters.AddWithValue("@AssignmentName", assignment.AssignmentName);
                updateCommand.Parameters.AddWithValue("@Description", assignment.Description);
                updateCommand.Parameters.AddWithValue("@AssignmentID", assignment.AssignmentID);
                updateCommand.ExecuteNonQuery();
            }
        }

        public void DeleteAssignment(int assignmentId)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var deleteCommand = new SqliteCommand(@"
                    DELETE FROM Assignments
                    WHERE AssignmentID = @AssignmentID;
                ", db);
                deleteCommand.Parameters.AddWithValue("@AssignmentID", assignmentId);
                deleteCommand.ExecuteNonQuery();
            }
        }

        // DEADLINES

        public void AddDeadline(Deadline deadline)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var insertCommand = new SqliteCommand(@"
                    INSERT INTO Deadlines (StudentID, AssignmentID, DeadlineDate, SubmissionDate)
                    VALUES (@StudentID, @AssignmentID, @DeadlineDate, @SubmissionDate);
                ", db);
                insertCommand.Parameters.AddWithValue("@StudentID", deadline.StudentID);
                insertCommand.Parameters.AddWithValue("@AssignmentID", deadline.AssignmentID);
                insertCommand.Parameters.AddWithValue("@DeadlineDate", deadline.DeadlineDate.ToString("yyyy-MM-dd")); // Format date for SQLite
                insertCommand.Parameters.AddWithValue("@SubmissionDate", deadline.SubmissionDate?.ToString("yyyy-MM-dd")); // Format nullable date
                insertCommand.ExecuteNonQuery();
            }
        }

        public ObservableCollection<Deadline> GetDeadlines(int studentId)
        {
            ObservableCollection<Deadline> deadlines = new ObservableCollection<Deadline>();

            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var selectCommand = new SqliteCommand(@"
                    SELECT DeadlineID, StudentID, AssignmentID, DeadlineDate, SubmissionDate
                    FROM Deadlines
                    WHERE StudentID = @StudentID;
                ", db);
                selectCommand.Parameters.AddWithValue("@StudentID", studentId);

                using (SqliteDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Deadline deadline = new Deadline();
                        deadline.DeadlineID = reader.GetInt32(0);
                        deadline.StudentID = reader.GetInt32(1);
                        deadline.AssignmentID = reader.GetInt32(2);
                        deadline.DeadlineDate = DateTime.Parse(reader.GetString(3)); // Parse date from string
                        if (!reader.IsDBNull(4))
                        {
                            deadline.SubmissionDate = DateTime.Parse(reader.GetString(4)); // Parse nullable date
                        }
                        deadlines.Add(deadline);
                    }
                }
            }

            return deadlines;
        }

        public void UpdateDeadline(Deadline deadline)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var updateCommand = new SqliteCommand(@"
                    UPDATE Deadlines
                    SET StudentID = @StudentID,
                        AssignmentID = @AssignmentID,
                        DeadlineDate = @DeadlineDate,
                        SubmissionDate = @SubmissionDate
                    WHERE DeadlineID = @DeadlineID;
                ", db);
                updateCommand.Parameters.AddWithValue("@StudentID", deadline.StudentID);
                updateCommand.Parameters.AddWithValue("@AssignmentID", deadline.AssignmentID);
                updateCommand.Parameters.AddWithValue("@DeadlineDate", deadline.DeadlineDate.ToString("yyyy-MM-dd")); // Format date for SQLite
                updateCommand.Parameters.AddWithValue("@SubmissionDate", deadline.SubmissionDate?.ToString("yyyy-MM-dd")); // Format nullable date
                updateCommand.Parameters.AddWithValue("@DeadlineID", deadline.DeadlineID);
                updateCommand.ExecuteNonQuery();
            }
        }

        public void DeleteDeadline(int deadlineId)
        {
            using (var db = new SqliteConnection($"Filename={_databasePath}"))
            {
                db.Open();

                var deleteCommand = new SqliteCommand(@"
                    DELETE FROM Deadlines
                    WHERE DeadlineID = @DeadlineID;
                ", db);
                deleteCommand.Parameters.AddWithValue("@DeadlineID", deadlineId);
                deleteCommand.ExecuteNonQuery();
            }
        }
    }
}