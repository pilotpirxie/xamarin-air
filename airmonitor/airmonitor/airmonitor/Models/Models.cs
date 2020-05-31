﻿using System;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace airmonitor.Models
{
    public struct Models
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string DisplayAddress1 { get; set; }
        public string DisplayAddress2 { get; set; }

        public string Description => $"{Street} {Number}, {City}";
    }

    public class AirQualityIndex
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public string Advice { get; set; }
        public string Color { get; set; }
    }

    public class AirQualityStandard
    {
        public string Name { get; set; }
        public string Pollutant { get; set; }
        public double Limit { get; set; }
        public double Percent { get; set; }
        public string Averaging { get; set; }
    }

    public class Installation
    {
        public string Id { get; set; }
        public Location Location { get; set; }
        public Models Address { get; set; }
        public double Elevation { get; set; }

        [JsonProperty(PropertyName = "airly")] public bool IsAirlyInstallation { get; set; }
    }

    public class Measurement
    {
        public int CurrentDisplayValue { get; set; }
        public MeasurementItem Current { get; set; }
        public MeasurementItem[] History { get; set; }
        public MeasurementItem[] Forecast { get; set; }
        public Installation Installation { get; set; }
    }

    public class MeasurementItem
    {
        public DateTime FromDateTime { get; set; }
        public DateTime TillDateTime { get; set; }
        public MeasurementValue[] Values { get; set; }
        public AirQualityIndex[] Indexes { get; set; }
        public AirQualityStandard[] Standards { get; set; }
    }

    public class MeasurementValue
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
}