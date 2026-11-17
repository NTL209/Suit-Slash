using System;
using System.Collections.Generic;
using System.Linq;

namespace Suit___Slash
{
    public enum BattleResult
    {
        Ongoing,
        Victory,
        Defeat
    }

    public sealed class PlayerBattleLogic
    {
        public int PlayerHp { get; private set; }
        public int BossHp { get; private set; }
        public BattleResult Result { get; private set; }

        public PlayerBattleLogic(
            int playerHp = 100,
            int bossHp = 1000)
        {
            if (playerHp < 0)
                throw new ArgumentOutOfRangeException(nameof(playerHp));

            if (bossHp < 0)
                throw new ArgumentOutOfRangeException(nameof(bossHp));

            PlayerHp = playerHp;
            BossHp = bossHp;

            CheckEndGame();
        }

        // Chọn bài → đánh bài → trừ máu Boss.
        public bool TryAttackBoss(
            IEnumerable<CardViewModel> playerHand,
            int damage)
        {
            if (Result != BattleResult.Ongoing ||
                playerHand == null ||
                damage <= 0)
            {
                return false;
            }

            bool hasSelectedCard = playerHand.Any(
                card => card != null && card.IsSelected);

            if (!hasSelectedCard)
                return false;

            BossHp = Math.Max(0, BossHp - damage);

            CheckEndGame();
            return true;
        }

        // Đồng bộ HP khi phần khác thay đổi máu.
        public void UpdateHealth(int playerHp, int bossHp)
        {
            if (Result != BattleResult.Ongoing)
                return;

            PlayerHp = Math.Max(0, playerHp);
            BossHp = Math.Max(0, bossHp);

            CheckEndGame();
        }

        public void UpdatePlayerHp(int currentHp)
        {
            UpdateHealth(currentHp, BossHp);
        }

        public BattleResult CheckEndGame()
        {
            if (BossHp <= 0)
                Result = BattleResult.Victory;
            else if (PlayerHp <= 0)
                Result = BattleResult.Defeat;

            return Result;
        }
    }
}
