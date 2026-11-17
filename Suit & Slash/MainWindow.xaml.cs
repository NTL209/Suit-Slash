using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Suit___Slash
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<CardViewModel> _playerHand = new ObservableCollection<CardViewModel>();

        public MainWindow()
        {
            InitializeComponent();
            LoadSampleCards();

            // Gắn sự kiện thử nghiệm cho nút Đánh bài và Bỏ bài
            btnPlayCard.Click += BtnPlayCard_Click;
            btnDiscard.Click += BtnDiscard_Click;
        }

        private void LoadSampleCards()
        {
            _playerHand = new ObservableCollection<CardViewModel>
            {
                new CardViewModel { Rank = "A", Suit = "♠", Color = "#212121" },
                new CardViewModel { Rank = "10", Suit = "♥", Color = "#D32F2F" },
                new CardViewModel { Rank = "K", Suit = "♦", Color = "#D32F2F" },
                new CardViewModel { Rank = "7", Suit = "♣", Color = "#212121" },
                new CardViewModel { Rank = "Q", Suit = "♥", Color = "#D32F2F" }
            };

            icPlayerHand.ItemsSource = _playerHand;
        }

        // Demo hiệu ứng khi bấm nút "Đánh bài"
        private void BtnPlayCard_Click(object sender, RoutedEventArgs e)
        {
            var selectedCards = _playerHand.Where(c => c.IsSelected).ToList();
            if (selectedCards.Count == 0)
            {
                txtHandStatus.Text = "Bạn chưa chọn lá bài nào để đánh!";
                return;
            }

            foreach (var card in selectedCards)
            {
                _playerHand.Remove(card);
            }
            txtHandStatus.Text = $"Đã đánh ra {selectedCards.Count} lá bài tấn công Boss!";
        }

        // Demo hiệu ứng khi bấm nút "Bỏ bài"
        private void BtnDiscard_Click(object sender, RoutedEventArgs e)
        {
            var selectedCards = _playerHand.Where(c => c.IsSelected).ToList();
            foreach (var card in selectedCards)
            {
                _playerHand.Remove(card);
            }
            txtHandStatus.Text = selectedCards.Count > 0
                ? $"Đã bỏ {selectedCards.Count} lá bài."
                : "Chọn lá bài muốn bỏ trước!";
        }
    }

    public class CardViewModel
    {
        public string Rank { get; set; } = "";
        public string Suit { get; set; } = "";
        public string Color { get; set; } = "#212121";
        public bool IsSelected { get; set; }
    }
}