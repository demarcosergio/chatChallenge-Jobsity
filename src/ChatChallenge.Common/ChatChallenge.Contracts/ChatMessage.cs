﻿namespace ChatChallenge.Contracts
{
    public record ChatMessage
    {
        public string UserName { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
        public DateTime MessageDateTime { get; set; }
    }
}