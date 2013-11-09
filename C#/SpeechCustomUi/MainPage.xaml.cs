//Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
//    Use of this sample source code is subject to the terms of the Microsoft license 
//    agreement under which you licensed this sample source code and is provided AS-IS.
//    If you did not accept the terms of the license agreement, you are not authorized 
//    to use this sample source code.  For the terms of the license, please see the 
//    license agreement between you and Microsoft.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Bing.Speech;

namespace SpeechCustomUi
{
    public sealed partial class MainPage : Page
    {
        private Event currentEvent = new Event();

        private Weather currentWeather = new Weather();
        private bool weatherq = false;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        SpeechRecognizer SR;
        bool cancelled;
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply credentials from the Windows Azure Data Marketplace.
            var credentials = new SpeechAuthorizationParameters();
            credentials.ClientId = "wiri";
            credentials.ClientSecret = "duI/DpDdGLaf9nshQzps0DWtoyHo8VWqZppXHm2Ykjs=";

            // Initialize the speech recognizer.
            SR = new SpeechRecognizer("en-US", credentials);

            // Add speech recognition event handlers.
            SR.AudioCaptureStateChanged += SR_AudioCaptureStateChanged;
            SR.AudioLevelChanged += SR_AudioLevelChanged;
            SR.RecognizerResultReceived += SR_RecognizerResultReceived;
        }


        void SR_RecognizerResultReceived(SpeechRecognizer sender,
            SpeechRecognitionResultReceivedEventArgs args)
        {
            if (args.Text == null) return;
            IntermediateResults.Text = args.Text;
        }

        void SR_AudioLevelChanged(SpeechRecognizer sender,
            SpeechRecognitionAudioLevelChangedEventArgs args)
        {
            var v = args.AudioLevel;
            if (v > 0) VolumeMeter.Opacity = v / 50;
            else VolumeMeter.Opacity = Math.Abs((v - 50) / 100);
        }

        void SR_AudioCaptureStateChanged(SpeechRecognizer sender,
            SpeechRecognitionAudioCaptureStateChangedEventArgs args)
        {
            // Show the panel that corresponds to the current state.
            switch (args.State)
            {
                case SpeechRecognizerAudioCaptureState.Complete:
                    if (!this.cancelled) SetPanel(CompletePanel);
                    break;
                case SpeechRecognizerAudioCaptureState.Initializing:
                    SetPanel(InitPanel);
                    break;
                case SpeechRecognizerAudioCaptureState.Listening:
                    SetPanel(ListenPanel);
                    break;
                case SpeechRecognizerAudioCaptureState.Thinking:
                    SetPanel(ThinkPanel);
                    break;
                default:
                    break;
            }
        }

        private void doTask(string text)
        {
            if(text.Contains("weather"))
            {
                currentWeather.get();
                weatherq = true;
            }
            else
                currentEvent.create(text, DateTimeOffset.Now);
        }
        private void SetPanel(StackPanel panel)
        {
            // Hide all the panels.
            InitPanel.Visibility = Visibility.Collapsed;
            ListenPanel.Visibility = Visibility.Collapsed;
            ThinkPanel.Visibility = Visibility.Collapsed;
            CompletePanel.Visibility = Visibility.Collapsed;
            StartPanel.Visibility = Visibility.Collapsed;

            // Show the selected panel and the cancel button.
            panel.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
        }
        
        private async void SpeakButton_Click(object sender, RoutedEventArgs e)
        {
            this.cancelled = false;
            AlternatesListBox.ItemsSource = null;

            // Use a try block because RecognizeSpeechToTextAsync depends on a web service.
            try
            {
                // Start speech recognition.
                var result = await SR.RecognizeSpeechToTextAsync();

                // Show the TextConfidence.
                if(weatherq)
                {
                    ConfidenceText.Text = Globals.weather;
                }
                else
                    ConfidenceText.Text = "";

                // Display the text.
                FinalResult.Text = result.Text;

                //place text in a Global Variable
                Globals.text = FinalResult.Text;

                // Fill a string array with the alternate results.
                var alternates = result.GetAlternates(5);
                doTask(Globals.text);
                if (alternates.Count > 1)
                {
                    string[] s = new string[alternates.Count];
                    for (int i = 1; i < alternates.Count; i++)
                    {
                        s[i] = alternates[i].Text;
                    }

                    // Populate the alternates ListBox with the array.
                    AlternatesListBox.ItemsSource = s;
                    AlternatesTitle.Visibility = Visibility.Visible;
                }
                else
                {
                    AlternatesTitle.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                // If there's an exception, show it in the Complete panel.
                if (ex.GetType() != typeof(OperationCanceledException))
                {
                    FinalResult.Text = string.Format("{0}: {1}",
                                ex.GetType().ToString(), ex.Message);
                    SetPanel(CompletePanel); 
                }
            }
        }

        private string getConfidence(SpeechRecognitionConfidence confidence)
        {
            switch (confidence)
            {
                case SpeechRecognitionConfidence.High:
                    return("I am at least 85% sure you said:");
                case SpeechRecognitionConfidence.Medium:
                    return("I am at least 70% sure you said:");
                case SpeechRecognitionConfidence.Low:
                    return("I think you might have said:");
                default:
                    return("I'm sorry, I couldn't understand you."
                    + " Please click the Cancel button and try again.");
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Cancel the current speech session and return to start.
            this.cancelled = true;
            CancelButton.Visibility = Visibility.Collapsed; 
            SR.RequestCancelOperation();
            SetPanel(StartPanel);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // Stop listening and move to Thinking state.
            SR.StopListeningAndProcessAudio();
        }

        private void AlternatesListBox_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            // Check in case the ListBox is still empty.
            if (null != AlternatesListBox.SelectedItem)
            {
                // Put the selected text in FinalResult and clear ConfidenceText.
                FinalResult.Text = AlternatesListBox.SelectedItem.ToString();
                ConfidenceText.Text = "";
            }
        }
    }
}
