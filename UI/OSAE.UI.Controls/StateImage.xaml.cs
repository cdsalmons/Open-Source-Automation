﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for StateImage.xaml  
    /// </summary>
    public partial class StateImage : UserControl
    {
        public OSAEObject screenObject { get; set; }
        public Point Location;
        public DateTime LastUpdated;
        public DateTime LastStateChange;
        
        public string StateMatch;
        public string CurState;
        public string CurStateLabel;
        public string CurLevel = "";
        public int LightLevel = 100;

        public string ObjectName;
        public string SliderMethod;
       
        private MemoryStream ms1;
        private MemoryStream ms2;
        private MemoryStream ms3;
        private MemoryStream ms4;
        private MemoryStream[] msArray;

        private int imageFrames = 0;
        private int currentFrame = 0;
        private int frameDelay = 100;
        private bool repeatAnimation;
        private Boolean sliderVisible = false;
        private Boolean updatingSlider = false; 
        private DispatcherTimer timer = new DispatcherTimer();

        public StateImage(OSAEObject sObject)
        {
            InitializeComponent();

            OSAEImageManager imgMgr = new OSAEImageManager();

            screenObject = sObject;
            ObjectName = screenObject.Property("Object Name").Value;
            SliderMethod = screenObject.Property("Slider Method").Value;
            CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;

            try
            { LightLevel = Convert.ToUInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, "Light Level").Value); }
            catch
            { LightLevel = 100; }

            LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;
            Image.ToolTip = ObjectName + "\n" + CurState + " since: " + LastStateChange;

            Image.Tag = ObjectName;
            Image.MouseLeftButtonUp += new MouseButtonEventHandler(State_Image_MouseLeftButtonUp);

            foreach (OSAEObjectProperty p in screenObject.Properties)
            {
                if (p.Value.ToLower() == CurState.ToLower())
                    StateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
            }

            string imgName = screenObject.Property(StateMatch + " Image").Value;
            string imgName2 = screenObject.Property(StateMatch + " Image 2").Value;
            string imgName3 = screenObject.Property(StateMatch + " Image 3").Value;
            string imgName4 = screenObject.Property(StateMatch + " Image 4").Value;
            try
            {
                repeatAnimation = Convert.ToBoolean(screenObject.Property("Repeat Animation").Value);
            }
            catch
            {
                OSAEObjectPropertyManager.ObjectPropertySet(screenObject.Name, "Repeat Animation", "TRUE", "GUI");
                repeatAnimation = true;
            }
            try
            { frameDelay = Convert.ToInt16(screenObject.Property("Frame Delay").Value); }
            catch
            {
                frameDelay = 100;
                OSAEObjectPropertyManager.ObjectPropertySet(screenObject.Name, "Frame Delay", "100", "GUI");
            }
            OSAEImage img1 = imgMgr.GetImage(imgName);
            if (img1 != null)
            {
                ms1 = new MemoryStream(img1.Data);
                BitmapImage bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms1;
                bitmapImage.EndInit();

                Image.Source = bitmapImage;
                Image.Visibility = System.Windows.Visibility.Visible;

                imageFrames = 1;
                currentFrame = 1;
                OSAEImage img2 = imgMgr.GetImage(imgName2);
                if (img2 != null)
                {
                    ms2 = new MemoryStream(img2.Data);
                    imageFrames = 2;
                    OSAEImage img3 = imgMgr.GetImage(imgName3);
                    if (img3 != null)
                    {
                        ms3 = new MemoryStream(img3.Data);
                        imageFrames = 3;
                        OSAEImage img4 = imgMgr.GetImage(imgName4);
                        if (img4 != null)
                        {
                            ms4 = new MemoryStream(img4.Data);
                            imageFrames = 4;
                        }
                    }
                }
            }
            else
            {
                Image.Source = null;
                Image.Visibility = System.Windows.Visibility.Hidden;
            }

            sliderVisible = Convert.ToBoolean(screenObject.Property("Show Slider").Value);

            if (sliderVisible)
            {
                sldSlider.Visibility = System.Windows.Visibility.Visible;
                CurLevel = OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, "Level").Value;
                sldSlider.Value = Convert.ToUInt16(CurLevel);
            }
            else
                sldSlider.Visibility = System.Windows.Visibility.Hidden;

            timer.Interval = TimeSpan.FromMilliseconds(frameDelay);
            timer.Tick += this.timer_Tick;

            if (imageFrames > 1) timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (currentFrame < imageFrames)
                currentFrame += 1;
            else if (currentFrame == imageFrames)
                currentFrame = 1;
            MemoryStream imageStream = new MemoryStream();
            
            switch (currentFrame)
            {
                case 1:
                    imageStream = ms1;
                    break;
                case 2:
                    imageStream = ms2;
                    break;
                case 3:
                    imageStream = ms3;
                    break;
                case 4:
                    imageStream = ms4;
                    break;
            }

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = imageStream;
            bitmapImage.EndInit();
            Image.Source = bitmapImage;
            Image.Visibility = System.Windows.Visibility.Visible;
        }

        public void Update()
        {
            OSAEImageManager imgMgr = new OSAEImageManager();
            bool stateChanged = false;
            try
            {
                OSAEObjectState stateCurrent = OSAEObjectStateManager.GetObjectStateValue(ObjectName);
                if (CurState != stateCurrent.Value) stateChanged = true;

                CurState = stateCurrent.Value;
                CurStateLabel = stateCurrent.StateLabel;
                LastStateChange = stateCurrent.LastStateChange;
            }
            catch
            { }

            foreach (OSAEObjectProperty p in screenObject.Properties)
            {
                if (p.Value.ToLower() == CurState.ToLower())
                    StateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
            }

            try
            {
                Location.X = Double.Parse(OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue(screenObject.Name, StateMatch + " X").Value);
                Location.Y = Double.Parse(OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue(screenObject.Name, StateMatch + " Y").Value);

                try
                {
                    LightLevel = Convert.ToUInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, "Light Level").Value);
                }
                catch
                { LightLevel = 100; }

                if (stateChanged)
                {
                    timer.Stop();
                    string imgName = screenObject.Property(StateMatch + " Image").Value;
                    string imgName2 = screenObject.Property(StateMatch + " Image 2").Value;
                    string imgName3 = screenObject.Property(StateMatch + " Image 3").Value;
                    string imgName4 = screenObject.Property(StateMatch + " Image 4").Value;

                    OSAEImage img1 = imgMgr.GetImage(imgName);
                    if (img1 != null)
                    {
                        ms1 = new MemoryStream(img1.Data);
                        imageFrames = 1;
                        currentFrame = 1;
                        // Primary Frame is loaded, load up additional frames for the time to display.
                        OSAEImage img2 = imgMgr.GetImage(imgName2);
                        if (img2 != null)
                        {
                            ms2 = new MemoryStream(img2.Data);
                            imageFrames = 2;
                            OSAEImage img3 = imgMgr.GetImage(imgName3);
                            if (img3 != null)
                            {
                                ms3 = new MemoryStream(img3.Data);
                                imageFrames = 3;
                                OSAEImage img4 = imgMgr.GetImage(imgName4);
                                if (img4 != null)
                                {
                                    ms4 = new MemoryStream(img4.Data);
                                    imageFrames = 4;
                                }
                            }
                        }
                    }
                    else
                    {
                        Image.Source = null;
                        Image.Visibility = System.Windows.Visibility.Hidden;
                        Image.Source = null;
                        Image.Visibility = System.Windows.Visibility.Hidden;
                    }

                    if (img1 != null)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms1;
                        bitmapImage.EndInit();
                        Image.Source = bitmapImage;

                        if (sliderVisible && updatingSlider == false)
                        {
                            CurLevel = OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, "Level").Value;
                            sldSlider.ToolTip = CurLevel + "%";
                            sldSlider.Value = Convert.ToUInt16(CurLevel);
                        }
                        if (CurLevel != "")
                            Image.ToolTip = ObjectName + "\n" + CurStateLabel + " (" + CurLevel + "%) since: " + LastStateChange;
                        else
                            Image.ToolTip = ObjectName + "\n" + CurStateLabel + " since: " + LastStateChange;
                    }));
                    }

                    if (imageFrames > 1) timer.Start();
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, "StateImage Update"); }
        }

        private void State_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {             
            if (CurState == "ON")
            {
                OSAEMethodManager.MethodQueueAdd(ObjectName, "OFF", "0", "", "GUI");
                OSAEObjectStateManager.ObjectStateSet(ObjectName, "OFF", "GUI");
            }
            else
            {
                OSAEMethodManager.MethodQueueAdd(ObjectName, "ON", "100", "", "GUI");
                OSAEObjectStateManager.ObjectStateSet(ObjectName, "ON", "GUI");
            }        
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (updatingSlider) return;
        }

        private void Slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            updatingSlider = true;
        }

        private void Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OSAEMethodManager.MethodQueueAdd(ObjectName, SliderMethod, Convert.ToUInt16(sldSlider.Value).ToString(), "", "GUI");
            updatingSlider = false;
        }        
    }
}