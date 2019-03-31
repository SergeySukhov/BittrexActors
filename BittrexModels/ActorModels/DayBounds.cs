using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BittrexModels.ActorModels
{
    public class DayBounds
    {
        DateTime StartBound { get; }
        TimeSpan Duration { get; }
        public readonly List<DayOfWeek> exceptionDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };

        public DayBounds(DateTime startBound, TimeSpan duration)
        {
            this.StartBound = startBound;
            this.Duration = duration;
        }

        public bool isInBounds()
        {
            return DateTime.Now.Subtract(this.StartBound) <= this.Duration && !exceptionDays.Contains(DateTime.Now.DayOfWeek);
        }

        public bool isInBounds(DateTime dateTime)
        {
            return dateTime.Subtract(this.StartBound) <= this.Duration && !exceptionDays.Contains(dateTime.DayOfWeek);
        }

    }
}
