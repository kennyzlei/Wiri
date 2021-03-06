﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Devices.Geolocation; 

namespace SpeechCustomUi
{
    public class Weather
    {
        //get weather at current location
        public async void get()
        {
            Geolocator locator = new Geolocator();
            Geoposition position = await locator.GetGeopositionAsync();
            String WeatherService = "http://weather.service.msn.com/data.aspx?weadegreetype=f&amp;culture=en-us&amp;weasearchstr=";
            XmlReader reader = XmlReader.Create(WeatherService + position.CivicAddress.PostalCode);

            reader.ReadToFollowing("current");
            reader.MoveToFirstAttribute();
            String temp = reader.Value;
            reader.MoveToNextAttribute();
            reader.MoveToNextAttribute();
            String sky = reader.Value;

            Globals.weather = "It is currently " + temp + "°F and " + sky;
        }
    }
}
