﻿using System;
namespace ProcCore.JqueryHelp.CustomButton
{
    public class jqButton : BaseJqueryScriptHelp, ijQueryUIScript
    {
        public jqButton(jqSelector ElementId)
        {
            this.jqId = ElementId;
            this.options = new Options();
        }
        public Options options { get; set; }
        public String ToScriptString()
        {
            MakeJqScript jqOptions = new MakeJqScript() { GetObject = this.options };
            return "$(" + this.jqId.ToOptionString() + ").button(" + jqOptions.MakeScript() + ");";
        }

        public class ICONS : BaseJqueryScriptHelp, ijQueryUIScript
        {
            public FrameworkIcons primary { get; set; }
            public FrameworkIcons secondary { get; set; }
            public String ToScriptString()
            {
                MakeJqScript jqOptions = new MakeJqScript() { GetObject = this, needBrace = false };
                return jqOptions.MakeScript();
            }
        }
        public class Options : BaseJqueryScriptHelp, ijQueryUIScript
        {
            public Options()
            {
                this.icons = new ICONS();
            }
            public Boolean? disabled { get; set; }
            public Boolean? text { get; set; }
            public String label { get; set; }
            public ICONS icons { get; set; }

            public String ToScriptString()
            {
                MakeJqScript jqOptions = new MakeJqScript() { GetObject = this, needBrace = false };
                return jqOptions.MakeScript();
            }
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                