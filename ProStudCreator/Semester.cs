using System;
namespace ProStudCreator
{
    public class Semester
    {
        private int semesterId;
        public static Semester CurrentSemester
        {
            get
            {
                return (Semester)DateTime.Now;
            }
        }
        public DateTime StartDate
        {
            get
            {
                return new DateTime(semesterId / 2, 2 + 6 * (semesterId & 1), 9);
            }
        }
        public DateTime EndDate
        {
            get
            {
                return (this + 1).StartDate.AddTicks(-1L);
            }
        }
        public static explicit operator Semester(DateTime _dt)
        {
            DateTime offset = _dt.AddMonths(-1).AddDays(-20.0);
            return new Semester
            {
                semesterId = _dt.Year * 2 + (_dt.Month-1) / 6
            };
        }
        public static Semester operator +(Semester _s, int _add)
        {
            return new Semester
            {
                semesterId = _s.semesterId + _add
            };
        }
        public static Semester operator -(Semester _s, int _add)
        {
            return _s + -_add;
        }
        public static Semester operator ++(Semester _s)
        {
            return _s + 1;
        }
        public static Semester operator --(Semester _s)
        {
            return _s - 1;
        }
        public static bool operator ==(Semester _lhs, Semester _rhs)
        {
            return ReferenceEquals(_lhs, _rhs) || (!ReferenceEquals(_lhs, null) && !ReferenceEquals(_rhs, null) && _lhs.semesterId == _rhs.semesterId);
        }
        public static bool operator ==(Semester _lhs, DateTime _rhs)
        {
            return _rhs >= _lhs.StartDate && _rhs <= _lhs.EndDate;
        }
        public static bool operator !=(Semester _lhs, Semester _rhs)
        {
            return !(_lhs == _rhs);
        }
        public static bool operator !=(Semester _lhs, DateTime _rhs)
        {
            return !(_lhs == _rhs);
        }
        public override bool Equals(object obj)
        {
            if (obj is Semester)
                return (this == (Semester)obj); //rely on overloaded == operator

            if(obj is DateTime)
                return (this == (DateTime)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return semesterId;
        }

        public int Year
        {
            get
            {
                return (semesterId+1) / 2;
            }
        }

        /// <summary>
        /// Returns "FS" or "HS"
        /// </summary>
        public string FSHS
        {
            get
            {
                if (((semesterId+1) & 1) == 0)
                    return "FS";
                else
                    return "HS";
            }
        }

        /// <summary>
        /// Returns HS15 for example
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FSHS+Year.ToString().Substring(2);
        }
    }
}
