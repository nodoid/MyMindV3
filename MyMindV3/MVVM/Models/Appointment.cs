using System;

namespace MvvmFramework.Models
{
    public class Appointment
    {
        public string AppointmentStatus { get; set; }

        public string AppointmentTime
        {
            get
            {
                return Convert.ToDateTime(AppointmentDateTime).ToString("hh:mm");
            }
        }

        public string ClientId { get; set; }

        public string NhsNo { get; set; }

        public string AppointmentDateTime { get; set; }

        public int? Duration { get; set; }

        public string Description { get; set; }

        public string Team { get; set; }

        public string AppointmentType { get; set; }

        public string Urgency { get; set; }

        public string AppointmentLocation { get; set; }

        public string AppointmentHcp { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string NextOfKin { get; set; }

        public string SequenceId { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string FullName { get; set; }

        public string FullClientName { get { return string.Format("{0} {1}", FirstName, Surname); } }

        public string Gender { get; set; }

        public string Title { get; set; }

        public string HomePhone { get; set; }

        public string EveningPhone { get; set; }

        public string MobilePhone { get; set; }

        public string AddressLine4 { get; set; }

        public string AddressLine5 { get; set; }

        public string Postcode { get; set; }

        public string Address { get; set; }

        public string AddressFull
        {
            get
            {
                return string.Format("{0}\n{1}\n{2}\n{3}\n{4}", AddressLine1, AddressLine2, AddressLine3, AddressLine4, AddressLine5);
            }
        }

        public string ContactNumbers { get; set; }

        public string DateOfBirth { get; set; }

        public string MainCarer { get; set; }

        public string Ethnicity { get; set; }

        public string Language { get; set; }

        public string InterpreterRequired { get; set; }

        public string RegisteredGp { get; set; }

        public string LookedAfterChild { get; set; }

        public string School { get; set; }

        public string InpatientStatus { get; set; }
    }
}
