using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ResoAV
{
    public class clCompetitor : INotifyPropertyChanged
    {
        public struct AttemptResult
        {
            public TimeSpan Time;
            public int PenaltyCount;
            public bool IsShod;
            public bool IsNotStarted;
            public override string ToString()
            {
                if (IsShod)
                    return "СХОД\n" + RaceSettings.RS.PenalityShod + "сек";
                if (IsNotStarted)
                    return "Не стартовал\n" + RaceSettings.RS.PenalityShod + "сек";
                if (PenaltyCount > 0)
                {
                    return String.Format("{0}\n+штраф {1} сек",
                        Time.ToResoAvView(),
                        PenaltyCount * RaceSettings.RS.Penality);
                }
                return Time.ToResoAvView();
            }
        }


		//public class carClass:System.Enum
		//{
		//    [Description("RB 1600")]
		//    STD2000 = 0,
		//    [Description("RB 2000")]
		//    FREE2000 = 1,
		//    [Description("RB 4000")]
		//    FREE4000 = 2,
		//    [Description("RB Absolute")]
		//    ABSOLUTE = 3,
		//    [Description("RB Sport")]
		//    SPORT = 4,

		//    public override string ToString()
		//    {
		//        return "Всегда одно и то же...";
		//    }
		//}

		public enum carClass
		{
			[Description("RB-Standart")]
			STANDART = 1,
			[Description("RB-Sport")]
			SPORT = 2,
            [Description("RB-4x4")]
            RB_4x4 = 3,
            [Description("RB-Tuning")]
			TUNING = 4,
		};

        public int Number { set; get; }
        public String Name { set; get; }
        public String Model { set; get; }
        public carClass Class { set; get; }

        //попытки массивом
        private AttemptResult[] _attemptCollection;
        public AttemptResult[] AttemptCollection 
        { 
            get 
            {
                if (_attemptCollection == null)
                    _attemptCollection = new AttemptResult[RaceSettings.RS.Attempts];
                return _attemptCollection; 
            } 
        }


        public void AttemptModify(int att_num, TimeSpan ts)
        {
            if (att_num < _attemptCollection.Length)
            {
                _attemptCollection[att_num].Time = ts;
                OnPropertyChanged("AttemptCollection");
            }
        }

        public void AttemptModify(int att_num, int penalty_increase)
        {
            if (att_num < _attemptCollection.Length)
            {
                _attemptCollection[att_num].PenaltyCount += penalty_increase;
                OnPropertyChanged("AttemptCollection");
            }
        }

        public void AttemptModifyShod(int att_num, bool shod)
        {
            if (att_num < _attemptCollection.Length)
            {
                AttemptCollection[att_num].IsNotStarted = !shod;
                AttemptCollection[att_num].IsShod = shod;
                OnPropertyChanged("AttemptCollection");
            }
        }

        public void AttemptModifyNS(int att_num, bool ns)
        {
            if (att_num < _attemptCollection.Length)
            {
                AttemptCollection[att_num].IsShod = !ns;
                AttemptCollection[att_num].IsNotStarted = ns;
                OnPropertyChanged("AttemptCollection");
            }
        }

		private bool _allowForRace(ref AttemptResult arg)
		{
			return !(arg.IsNotStarted || arg.IsShod || arg.Time != TimeSpan.Zero);
		}

		public bool AllowForRace(int attempt)
		{
            if (attempt < _attemptCollection.Length)
                return _allowForRace(ref AttemptCollection[attempt]);
            return false;
		}

		public TimeSpan Result
        {
            get
            {
                /*TimeSpan ts = new TimeSpan();
                foreach (var att in AttemptCollection)
                    ts.Add(GetSpanByAttempt(att));
                return ts;*/
                //return ResultCalculator(AttemptCollection);
                return RaceSettings1.CalcResult(this);
            }
        }

        public static TimeSpan GetSpanByAttempt(AttemptResult att)
        {
            if (att.IsShod || att.IsNotStarted)
                return new TimeSpan(0, 0, RaceSettings.RS.PenalityShod);
            else
                return att.Time.Add(new TimeSpan(0, 0, att.PenaltyCount * RaceSettings.RS.Penality));
        }

        public string TotalPenality { get { return "n/a"; } }


        public clCompetitor()
        {
        }

        public override string ToString()
        {
            return String.Format("{0} {1} ({3}) {2}", Number.ToString(), Name, Class, Model);
        }

        /*public delegate TimeSpan ResultCalculatorType(AttemptResult[] attempts);
        static public ResultCalculatorType ResultCalculator;
        static public TimeSpan OneBest(AttemptResult[] attempts)
        {
            if (attempts.Length == 0)
                return new TimeSpan();
            var res = attempts[0].Time;
            foreach(var ts in attempts)
            {
                if (ts.Time < res)
                    res = ts.Time;
            }
            return res;
        }
        
        static public TimeSpan SumTwoBest(AttemptResult[] attempts)
        {
            return new TimeSpan();
        }

        static public TimeSpan SumAll(AttemptResult[] attempts)
        {
            var res = new TimeSpan();
            foreach (var ts in attempts)
                res.Add(ts.Time);
            return res;
        }*/


        #region Члены INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }

}
