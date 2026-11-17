using System;
using System.Collections.Generic;
using System.Linq;

public enum Suit
{
    Co,
    Ro,
    Chuon,
    Bich
}

public class CardState
{
    public int Value { get; set; }
    public Suit suit { get; set; } // Khớp đúng với code của bạn
}

public enum HandRank
{
    HighCard = 1,
    Pair = 2,
    TwoPair = 3,
    ThreeOfAKind = 4,
    Straight = 5,
    Flush = 6,
    FullHouse = 7,
    FourOfAKind = 8,
    StraightFlush = 9
}

// Thêm class ScoredHand để lưu trữ kết quả đánh giá tay bài
public class ScoredHand
{
    public List<CardState> Cards { get; set; }
    public HandRank Rank { get; set; }
    public int TotalDamageScore { get; set; }
}