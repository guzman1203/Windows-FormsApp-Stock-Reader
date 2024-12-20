using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COP4365_Stock_Reader_2024;

namespace WindowsFormsApp_COP_4365_Stock_Reader_2024
{
    internal class Smart_CandleStick : CandleStick
    {
        // class variables
        public double range { get; set; }
        public double bodyRange { get; set; }
        public double topPrice { get; set; }
        public double bottomPrice { get; set; }
        public double upperTail { get; set; }
        public double lowerTail { get; set; }

        static private readonly double gn_p = 0.075; // percent of range to be considered not existing

        // pattern properties
        // patterns[number of cs][pattern name]
        public Dictionary<string, Dictionary<string, bool>> patterns = new Dictionary<string, Dictionary<string, bool>>();

        // keys of the dictionary can be used as a combobox to tell the user which patterns your app recognizes
    
        //
        // constructors
        //

        // default
        public Smart_CandleStick() : base()
        {
            ticker = "";
            date = DateTime.Now;
            period = "";
            range = 0;
            bodyRange = 0;
            topPrice = 0;
            bottomPrice = 0;
            upperTail = 0;
            lowerTail = 0;
            computePatternProperties();
        }
        // copy
        public Smart_CandleStick(CandleStick cs)
        {
            this.ticker = cs.ticker;
            this.period = cs.period;
            this.date = cs.date;
            this.high = cs.high;
            this.low = cs.low;
            this.open = cs.open;
            this.close = cs.close;
            this.volume = cs.volume;
            computeExtraProperties();
            computePatternProperties();
        }
        // base
        public Smart_CandleStick(string cvs_input, string cvs_filename) : base(cvs_input, cvs_filename) 
        { 
            computeExtraProperties();
            computePatternProperties();
        }
        /// <summary>
        /// compute the extra properties of a smart candlestick
        /// </summary>
        private void computeExtraProperties()
        {
            range = this.high - this.low;
            bodyRange = Math.Abs(this.open - this.close);
            topPrice = Math.Max(this.open, this.close);
            bottomPrice = Math.Min(this.close, this.open);
            upperTail = Math.Abs(this.high - topPrice);
            lowerTail = Math.Abs(this.bottomPrice - this.low);
        }
        private void computePatternProperties()
        {
            computeSinglePatternProperties();
            computeDoublePatternProperties();
            computeTriplePatternProperties();
        }
        /// <summary>
        /// Adds to the patterns dictionary single patterns and determines if this candlestick is of a given pattern
        /// </summary>
        private void computeSinglePatternProperties()
        {
            patterns.Add("Single", new Dictionary<string, bool>());
            patterns["Single"].Add("isBearish", isBearish());
            patterns["Single"].Add("isBullish", isBullish());
            patterns["Single"].Add("isNeutral", this.bodyRange <= 0.01);

            patterns["Single"].Add("isMarobuzo", isMarubozu());

            patterns["Single"].Add("isHammer", isHammer());
            patterns["Single"].Add("isInvHammer", isInvHammer());

            patterns["Single"].Add("isDoji", isDoji());
            patterns["Single"].Add("isDragonflyDoji", isDragonflyDoji());
            patterns["Single"].Add("isGravestoneDoji", isGravestoneDoji());

            patterns["Single"].Add("isHangingman", isHangingman());

            //patterns["Single"].Add("", ());
        }
        /// <summary>
        /// Adds to the patterns dictionary double patterns and determines if this candlestick is of a given pattern
        /// </summary>
        private void computeDoublePatternProperties()
        {
            patterns.Add("Double", new Dictionary<string, bool>());
        }
        /// <summary>
        /// Adds to the patterns dictionary tripple patterns and determines if this candlestick is of a given pattern
        /// </summary>
        private void computeTriplePatternProperties()
        {
            patterns.Add("Triple", new Dictionary<string, bool>());
        }

        //
        // candle stick patterns determination methods
        //

        // Bullish
        // close > open
        private bool isBullish()
        {
            return this.close > this.open;
        }
        // Bearish
        // open > close. End
        private bool isBearish()
        {
            return this.open > this.close;
        }

        // Hammmer
        // body between 10% to 40% of range
        // upper tail 1% or less of range
        // lower tail > body range
        private bool isHammer()
        {
            double mx_br = 0.4*this.range;
            double mn_br = 0.1*this.range;
            
            bool isHammer = true;
            if (!(this.bodyRange >= mn_br &  this.bodyRange <= mx_br)) { isHammer  = false; } // bodyrange
            if (!(this.upperTail <= gn_p * this.range)) { isHammer = false; } // upper tail
            if (!(this.lowerTail > this.bodyRange)) { isHammer = false; } // lower tail
            return isHammer;
        }

        // Inverse hammer
        // body between 10% to 40% of range
        // bottom tail 20% or less of range
        // upper tail > body range
        private bool isInvHammer()
        {
            bool isHammer = true;
            double mx_br = 0.4 * this.range;
            double mn_br = 0.1 * this.range;
            
            if (!(this.bodyRange >= mn_br & this.bodyRange <= mx_br)) { isHammer = false; } // body range range
            if (!(this.upperTail > this.bodyRange)) { isHammer = false; } // upper tail is greater than lower tail
            if (!(this.lowerTail <= gn_p * this.range)) { isHammer = false; } // lower tail is small or missing
            return isHammer;
        }

        // Hanging man
        // isBearish
        // lowerTail >= 2*bodyRange
        // upperTail 5% or less of range
        private bool isHangingman()
        {
            bool isHangingman = true;
            
            if (!(this.patterns["Single"]["isBearish"])) { isHangingman = false; } // cs is bearish
            else if (!(this.lowerTail >= 2*this.bodyRange)) { isHangingman = false; } // lower tail is twice as big as body range
            else if (!(upperTail <= gn_p * this.range)) { isHangingman = false; } // upper tail is small or missing

            return isHangingman;
        }

        // Doji
        // top price ~= bottom price
        private bool isDoji()
        {
            bool isDoji = true;

            if (!((this.bodyRange) <= gn_p)) { isDoji = false; }

            return isDoji;
        }

        // Doji Dragonfly
        // is a Doji
        // lower tail >= 2/3 range 
        private bool isDragonflyDoji()
        {
            bool isDragonflyDoji = true;

            if (!isDoji()) { isDragonflyDoji = false; }
            else if (!(this.lowerTail >= ((2.0/3.0) * this.range))) { isDragonflyDoji = false; }
            //else if (!(this.upperTail <= ((1/3) * this.range))) { isDragonflyDoji = false; }

            return isDragonflyDoji;
        }

        // Doji Gravestone
        // is a Doji
        // upper tail >= 2/3 range
        private bool isGravestoneDoji()
        {
            bool isGravestoneDoji = true;

            if (!isDoji()) { isGravestoneDoji = false; }
            else if (!(upperTail >= ((2.0/3.0) * range))) { isGravestoneDoji = false; }
            //else if (!(this.lowerTail <= ((1/3) * this.range))) { isGravestoneDoji = false; }

            return isGravestoneDoji;
        }

        // Marubozu "shaved head"
        // bodyrange >= 90% of range
        private bool isMarubozu()
        {
            bool isMarubozu = true;

            if (!(this.lowerTail <= gn_p * this.range)) { isMarubozu=false; }
            else if (!(this.upperTail <= gn_p * this.range)) { isMarubozu=false; }

            return isMarubozu;
        }

    }
}
