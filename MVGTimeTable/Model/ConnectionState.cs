// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace MVGTimeTable.Model
{
    internal enum ConnectionState
    {
        JUST_STARTED,
        NOT_CONNECTED,
        CONNECTED,
        SHORT_DISCONNECT,
        LONG_DISCONNECT
    }
}