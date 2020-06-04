using NServiceBus;

namespace SharedMessages
{
    public class SomethingHappened : IEvent
    {
        public string WhatHappened { get; set; }
    }
}