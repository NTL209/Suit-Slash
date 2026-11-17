using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Suit___Slash
{
    public partial class MainWindow
    {
        // Công thức demo: mỗi lá gây 200 sát thương.
        private const int DemoDamagePerCard = 200;

        private PlayerBattleLogic _playerBattle;
        private bool _updatingBattleUi;

        static MainWindow()
        {
            // Chạy trước sự kiện Click của bản demo.
            EventManager.RegisterClassHandler(
                typeof(Button),
                ButtonBase.ClickEvent,
                new RoutedEventHandler(HandlePlayerBattleClick));
        }

        private static void HandlePlayerBattleClick(
            object sender,
            RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button == null)
                return;

            var window = Window.GetWindow(button) as MainWindow;

            // Chỉ xử lý nút Đánh bài của cửa sổ này.
            if (window == null ||
                !ReferenceEquals(button, window.btnPlayCard))
            {
                return;
            }

            // Ngăn handler demo xóa bài trước khi tính sát thương.
            e.Handled = true;

            window.PlaySelectedCards();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            EnsurePlayerBattleInitialized();
        }

        private void EnsurePlayerBattleInitialized()
        {
            if (_playerBattle != null)
                return;

            _playerBattle = new PlayerBattleLogic(
                (int)pbPlayerHp.Value,
                (int)pbBossHp.Value);

            // Kiểm tra kết thúc khi một trong hai thanh HP đổi.
            pbPlayerHp.ValueChanged += BattleHealthChanged;
            pbBossHp.ValueChanged += BattleHealthChanged;

            RefreshPlayerBattleUi();
        }

        private void PlaySelectedCards()
        {
            EnsurePlayerBattleInitialized();

            if (_playerBattle.Result != BattleResult.Ongoing)
                return;

            var selectedCards = _playerHand
                .Where(card => card.IsSelected)
                .ToList();

            if (selectedCards.Count == 0)
            {
                txtHandStatus.Text =
                    "Bạn chưa chọn lá bài nào để đánh!";

                return;
            }

            // Thay dòng này bằng kết quả tính điểm bài sau này.
            int damage = selectedCards.Count * DemoDamagePerCard;

            bool success = _playerBattle.TryAttackBoss(
                selectedCards,
                damage);

            if (!success)
                return;

            // Chỉ xóa bài sau khi tấn công thành công.
            foreach (var card in selectedCards)
            {
                _playerHand.Remove(card);
            }

            txtHandStatus.Text =
                $"Đã đánh {selectedCards.Count} lá, " +
                $"gây {damage} sát thương cho Boss!";

            RefreshPlayerBattleUi();
        }

        private void BattleHealthChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (_updatingBattleUi || _playerBattle == null)
                return;

            _playerBattle.UpdateHealth(
                (int)pbPlayerHp.Value,
                (int)pbBossHp.Value);

            RefreshPlayerBattleUi();
        }

        // Phần khác gọi hàm này khi HP người chơi thay đổi.
        public void UpdatePlayerHealth(int currentHp)
        {
            Dispatcher.VerifyAccess();
            EnsurePlayerBattleInitialized();

            _playerBattle.UpdatePlayerHp(
                Math.Min(currentHp, (int)pbPlayerHp.Maximum));

            RefreshPlayerBattleUi();
        }

        private void RefreshPlayerBattleUi()
        {
            // Tránh sự kiện ValueChanged gọi lặp khi cập nhật HP.
            _updatingBattleUi = true;

            try
            {
                pbBossHp.Value = _playerBattle.BossHp;
                pbPlayerHp.Value = _playerBattle.PlayerHp;

                txtBossHp.Text =
                    $"HP: {_playerBattle.BossHp} / " +
                    $"{pbBossHp.Maximum:0}";

                txtPlayerHp.Text =
                    $"HP Người chơi: {_playerBattle.PlayerHp} / " +
                    $"{pbPlayerHp.Maximum:0}";
            }
            finally
            {
                _updatingBattleUi = false;
            }

            if (_playerBattle.Result == BattleResult.Ongoing)
                return;

            bool won =
                _playerBattle.Result == BattleResult.Victory;

            txtHandStatus.Text = won
                ? "BẠN ĐÃ THẮNG!"
                : "BẠN ĐÃ THUA!";

            txtHandStatus.Foreground = won
                ? Brushes.Gold
                : Brushes.OrangeRed;

            // Trận đã kết thúc thì khóa các thao tác.
            icPlayerHand.IsEnabled = false;
            btnPlayCard.IsEnabled = false;
            btnDrawCard.IsEnabled = false;
            btnDiscard.IsEnabled = false;
            btnEndTurn.IsEnabled = false;
            btnSurrender.IsEnabled = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_playerBattle != null)
            {
                pbPlayerHp.ValueChanged -= BattleHealthChanged;
                pbBossHp.ValueChanged -= BattleHealthChanged;
            }

            base.OnClosed(e);
        }
    }
}
