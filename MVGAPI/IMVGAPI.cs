// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace MVGAPI
{
    public interface IMVGAPI
    {
        bool IsConnected { get; set; }
        List<string> StationsList { get; }

        void DeleteDuplicates(ref DeserializedDepartures[] deserializedDepartures);
        void FormatNewAPItoOld(ref DeserializedDepartures[] deserializedDepartures);
        DeserializedDepartures[] GetDeserializedDepartures(string stationID);
        string GetIdForStation(string stationName);
        string GetJsonDepartures(string stationID);
        string PerformApiRequest(string url, int requestTimeOut = 15000);
        void Sort(ref DeserializedDepartures[] deserializedDepartures);
    }
}