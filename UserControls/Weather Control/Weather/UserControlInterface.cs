﻿using System;
using System.Data;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSAE;
using PluginInterface;

namespace OSAE.Weather_Control
{
    public class UserControlInterface : IPlugin
    {
        public UserControlInterface()
		{
			
		}

        //Declarations of all our internal plugin variables
        string myName = "WEATHERCONTROL";
        string myDescription = "Weather Screen Control";
        string myAuthor = "Brian - Automate";
        string myVersion = "0.4.0";

        #region DO NOT CHANGE
        IPluginHost myHost = null;
        System.Windows.Controls.UserControl AddCtrlInterface;
        System.Windows.Controls.UserControl MainCtrl;
        public string Description { get { return myDescription; } }
        public string Author { get { return myAuthor; } }
        public IPluginHost Host { get { return myHost; } set { myHost = value; } }
        public string Name { get { return myName; } }
        public System.Windows.Controls.UserControl CtrlInterface { get { return AddCtrlInterface; } }
        public System.Windows.Controls.UserControl mainCtrl { get { return MainCtrl; } }
        public string Version { get { return myVersion; } }
        public void InitializeAddCtrl(string screen, string pluginName, string obj) { AddCtrlInterface = new AddNewControl(screen, myName, obj); }
        public void InitializeMainCtrl(OSAEObject obj)
        {
            MainCtrl = new CustomUserControl(obj, myName);
        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }
        #endregion
    }
}
