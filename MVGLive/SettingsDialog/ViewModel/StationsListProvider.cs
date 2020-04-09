// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using AutoCompleteTextBox.Editors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;

namespace MVGLive
{
    /// <summary>
    /// 
    /// </summary>
    public class StationsListProvider : ISuggestionProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> StationsList { get; }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public StationsListProvider(List<string> stations)
        {
            if (stations != null)
            {
                StationsList = stations;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable GetSuggestions(string filter)
        {
            if(string.IsNullOrEmpty(filter) || filter.Length < 3)
            {
                return null;
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("de-De");
                return StationsList.Where(station => station.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
        }
    }
}
