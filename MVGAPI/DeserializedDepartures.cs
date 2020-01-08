namespace MVGAPI
{
    public class DeserializedDepartures
    {
        public long departureTime { get; set; }
        public string product { get; set; }
        public string label { get; set; }
        public string destination { get; set; }
        public bool live { get; set; }
        public long delay { get; set; }
        public bool canceled { get; set; }
        public string lineBackgroundColor { get; set; }
        public string departureId { get; set; }
        public bool sev { get; set; }
        public string platform { get; set; }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                return ((DeserializedDepartures)obj).departureTime == departureTime &&
                        string.Compare(((DeserializedDepartures)obj).destination, destination) == 0 &&
                        string.Compare(((DeserializedDepartures)obj).departureId, destination) == 0;
            }
        }

        public override int GetHashCode()
        {
            if (!string.IsNullOrEmpty(destination) && !string.IsNullOrEmpty(departureId))
            {
                return departureTime.GetHashCode() + destination.GetHashCode() + departureId.GetHashCode();
            }
            else
                return 0;
        }
    }
}
