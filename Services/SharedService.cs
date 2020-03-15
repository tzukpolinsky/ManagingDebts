using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Services
{
    public class SharedService
    {
        public long GetCurrectTimeSpanTicksByString(string timeSpanString)
        {
            long timeTicks = 0;
            switch (timeSpanString.Length)
            {
                case 1:
                    timeTicks = TimeSpan.Parse("00:00:0" + timeSpanString).Ticks;
                    break;
                case 2:
                    timeTicks = TimeSpan.Parse("00:00:" + timeSpanString).Ticks;
                    break;
                case 4:
                    timeTicks = TimeSpan.Parse("00:0" + timeSpanString).Ticks;
                    break;
                case 5:
                    timeTicks = TimeSpan.Parse("00:" + timeSpanString).Ticks;
                    break;
                case 7:
                    timeTicks = TimeSpan.Parse("0" + timeSpanString).Ticks;
                    break;
                case 8:
                    int amountOfHours =Convert.ToInt32(timeSpanString.Substring(0,2));
                    if (amountOfHours>24)
                    {
                        timeSpanString = timeSpanString.Replace(amountOfHours.ToString(), (amountOfHours - 24).ToString());
                        timeTicks = TimeSpan.Parse("23:59:59").Ticks;
                    }
                    timeTicks += TimeSpan.Parse(timeSpanString).Ticks;
                    break;
            }
            return timeTicks;
        }
        public IConfigurationRoot GetSystemSettings()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
            return builder.Build();
        }
    }
}
