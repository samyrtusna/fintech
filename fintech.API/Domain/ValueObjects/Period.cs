namespace fintech.API.Domain.ValueObjects
{
    public sealed record Period
    {
        public DateOnly StartDate { get;}
        public DateOnly EndDate { get; }

        public Period(DateOnly startDate, DateOnly endDate)
        {
            if (endDate < startDate)
            {
                throw new ArgumentException("End date cannot be earlier than start date.");
            }
            StartDate = startDate;
            EndDate = endDate;
        }

        public bool Contains(DateOnly date)
        {
            return date >= StartDate && date <= EndDate;
        }

        public int DurationInDays => EndDate.DayNumber - StartDate.DayNumber + 1;

        public static Period ForMonth(int year, int month)
        {
            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return new Period(startDate, endDate);
        }

        public static Period ForYear(int year)
        {
            return new Period(
                new DateOnly(year, 1, 1),
                new DateOnly(year, 12, 31)
            );
        }


    }
}
