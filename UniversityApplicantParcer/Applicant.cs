namespace UniversityApplicantParcer
{
    internal class Applicant
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public char Priority { get; set; }
        public bool IsChoose { get; set; }
        public bool IsDocument { get; set; }
        public float Rating { get; set; }

        public Applicant(uint id, string name, string status, char priority,
            bool ischoose, bool isdocument, float rating)
        {
            Id = id;
            Name = name;
            Status = status;
            Priority = priority;
            IsChoose = ischoose;
            IsDocument = isdocument;
            Rating = rating;
        }

        public void Print() => Console.WriteLine($"Id - {Id}\nName - {Name}\nStatus - {Status}\n" +
            $"Priority - {Priority}\nChoose University - {IsChoose}\nGive Document - {IsDocument}\n" +
            $"Rating - {Rating}");
    }
}
