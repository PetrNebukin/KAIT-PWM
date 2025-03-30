using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWorksManager.Models
{
    public class Deadline
    {
        public int DeadlineID { get; set; }
        public int StudentID { get; set; }
        public int AssignmentID { get; set; }
        public DateTime DeadlineDate { get; set; }
        public DateTime? SubmissionDate { get; set; } // Nullable DateTime
    }
}