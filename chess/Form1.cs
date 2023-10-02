using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace chess
{
    public partial class Form1 : Form
    {
        List<List<int>> chessBoard = new List<List<int>>()
        {
            new List<int> {25, 24, 23, 22, 21, 23, 24, 25}, // 6 = pijun, 5 = top, 4 = konj, 3 = lovac, 2 = dama, 1 = kralj, 0 = none
            new List<int> {26, 26, 26, 26, 26, 26, 26, 26}, // 1 ispred oznacava belu boju, 2 ispred oznacava crni boju
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> {16, 16, 16, 16, 16, 16, 16, 16},
            new List<int> {15, 14, 13, 12, 11, 13, 14, 15}
        };
        List<List<PictureBox>> pictureBoxes = new List<List<PictureBox>>();
        List<Point> knightCombinations = new List<Point>()
            {
                new Point(2, -1),
                new Point(2, 1),
                new Point(1, 2),
                new Point(-1, 2),
                new Point(-2, 1),
                new Point(-2, -1),
                new Point(-1, -2),
                new Point(1, -2),
            };
        List<Point> kingCombinations = new List<Point>()
            {
                new Point(1, 0),
                new Point(1, 1),
                new Point(0, 1),
                new Point(-1, 1),
                new Point(-1, 0),
                new Point(-1, -1),
                new Point(0, -1),
                new Point(1, -1),
            };
        List<PictureBox> circles = new List<PictureBox>();
        List<Point> possibleMovements = new List<Point>();

        string picturesFolderLocation = "pieces2/";
        int pieceSize = 75;
        int playerTurn = 1;
        Point LastClick;

        public Form1()
        {
            InitializeComponent();
            pictureBoxes = generatePictureBoxes();
            populateChessBoards();
        }

        private List<List<PictureBox>> generatePictureBoxes()
        {
            List<List<PictureBox>> temp = new List<List <PictureBox>> ();
            for (int i = 0; i < 8; i++)
            {
                temp.Add(new List<PictureBox> { null, null, null, null, null, null, null, null });
            }
            return temp;
        }

        private void populateChessBoards()
        {
            int i = 0;
            foreach (List<int> list in chessBoard)
            {
                foreach (int piece in list)
                {
                    createAndPlacePiece(piece, findLocation(i));
                    i++;
                }
            }
        }

        private void createAndPlacePiece(int pieceNumber, Point pieceLocation)
        {
            PictureBox picture = new PictureBox
            {
                Name = $"{pieceNumber}",
                BackColor = Color.Transparent,
                Location = pieceLocation,
                Size = new Size(pieceSize, pieceSize),
                ImageLocation = $"{picturesFolderLocation}{pieceNumber}.png"
            };
            pictureBox1.Controls.Add(picture);
            picture.MouseClick += new MouseEventHandler(pieceClicked);
            pictureBoxes[pieceLocation.Y / pieceSize][pieceLocation.X / pieceSize] = picture;
        }

        private Point findLocation(int pieceNumber)
        {
            int y = pieceNumber / chessBoard[0].Count;
            int x = pieceNumber - (y * chessBoard[0].Count);
            Point location = new Point(x * pieceSize, y * pieceSize);
            return location;
        }

        private void pieceClicked(object sender, EventArgs e)
        {
            PictureBox clickedPiece = sender as PictureBox;
            int x = clickedPiece.Location.X / pieceSize;
            int y = clickedPiece.Location.Y / pieceSize;
            LastClick = new Point(x, y);
            removeCircles();

            if (playerTurn == 1)
            {
                switch (clickedPiece.Name)
                {
                    case "16":
                        possibleMovements = findPawnMovements(x, y - 1, 1);
                        createAndPlaceCircles(possibleMovements);
                        break;
                    case "15":
                        possibleMovements = findRockMovements(x, y, 1);
                        createAndPlaceCircles(possibleMovements);
                        break;
                    case "14":
                        possibleMovements = findMovementsWithCombinations(x, y, 1, knightCombinations);
                        createAndPlaceCircles(possibleMovements);
                        break;
                    case "13":
                        possibleMovements = findBishopMovements(x, y, 1);
                        createAndPlaceCircles(possibleMovements);
                        break;
                    case "12":
                        possibleMovements = findQueenMovements(x, y, 1);
                        createAndPlaceCircles(possibleMovements);
                        break;
                    case "11":
                        possibleMovements = findMovementsWithCombinations(x, y, 1, kingCombinations);
                        createAndPlaceCircles(possibleMovements);
                        break;
                }
            }
            if (playerTurn == 2)
            {
                switch (clickedPiece.Name)
                {
                    case "26":
                        possibleMovements = findPawnMovements(x, y + 1, 2);
                        createAndPlaceCircles(possibleMovements);
                        break;
                    case "25":
                        possibleMovements = findRockMovements(x, y, 2);
                        createAndPlaceCircles(possibleMovements);
                        break;
                    case "24":
                        possibleMovements = findMovementsWithCombinations(x, y, 2, knightCombinations);
                        createAndPlaceCircles(possibleMovements);
                        break;
                    case "23":
                        possibleMovements = findBishopMovements(x, y, 2);
                        createAndPlaceCircles(possibleMovements);
                        break;

                    case "22":
                        possibleMovements = findQueenMovements(x, y, 2);
                        createAndPlaceCircles(possibleMovements);
                        break;
                    case "21":
                        possibleMovements = findMovementsWithCombinations(x, y, 2, kingCombinations);
                        createAndPlaceCircles(possibleMovements);
                        break;
                }
            }
        }

        private void createAndPlaceCircles(List<Point> possibleMovements)
        {
            foreach (Point pieceLocation in possibleMovements)
            {
                PictureBox circle = new PictureBox
                {
                    Name = $"{pieceLocation.X}{pieceLocation.Y}",
                    BackColor = Color.Transparent,
                    Location = new Point(0, 0),
                    Size = new Size(pieceSize, pieceSize),
                    ImageLocation = $"{picturesFolderLocation}4.png",
                };
                circle.MouseClick += new MouseEventHandler(circleClicked);
                pictureBoxes[pieceLocation.Y][pieceLocation.X].Controls.Add(circle);
                circles.Add(circle);
            }
        }

        private List<Point> findPawnMovements(int x, int y, int color)
        {
            int temp;
            if (color==2) {temp = 1;} else {temp = 0;}

            List<Point> points = new List<Point>();
            if (y >= 0 & y <= 7)
            {
                if (chessBoard[y][x] / 10 != color & chessBoard[y][x] == 0) {points.Add(new Point(x, (y)));  }
                if (color == 1 & y == 5 & chessBoard[y][x] == 0) {points.Add(new Point(x, (y - 1)));}
                if (color == 2 & y == 2) { points.Add(new Point(x, (y + 1)));}
            }
            if (x >= 1 & y >= 0  & x <= 7 & y <= 6 + temp)
            {
                if (chessBoard[y][x - 1] != 0 & chessBoard[y][x - 1] / 10 != color) {points.Add(new Point((x - 1), (y)));}
            }
            if (x >= 0 & y >= 0 & x <= 6 & y <= 6 + temp)
            {
                if (chessBoard[y][x + 1] != 0 & chessBoard[y][x + 1] / 10 != color) {points.Add(new Point((x + 1), (y)));}
            }
            return points;
        }

        private List<Point> findRockMovements(int x, int y, int color)
        {
            List<Point> points = new List<Point>();
            for (int i = x + 1; i <= 7; i++)
            {
                if (chessBoard[y][i] == 0)
                {
                    points.Add(new Point(i, y));
                }
                else
                {
                    if (color != chessBoard[y][i] / 10)
                    {
                        points.Add(new Point(i, y));
                    }
                    break;
                }
            }
            for (int i = x - 1; i >= 0; i--)
            {
                if (chessBoard[y][i] == 0)
                {
                    points.Add(new Point(i, y));
                }
                else
                {
                    if (color != chessBoard[y][i] / 10)
                    {
                        points.Add(new Point(i, y));
                    }
                    break;
                }
            }
            for (int i = y - 1; i >= 0; i--)
            {
                if (chessBoard[i][x] == 0)
                {
                    points.Add(new Point(x, i));
                }
                else
                {
                    if (color != chessBoard[i][x] / 10)
                    {
                        points.Add(new Point(x, i));
                    }
                    break;
                }
            }
            for (int i = y + 1; i <= 7; i++)
            {
                if (chessBoard[i][x] == 0)
                {
                    points.Add(new Point(x, i));
                }
                else
                {
                    if (color != chessBoard[i][x]/10)
                    {
                        points.Add(new Point(x, i));
                    }
                    break;
                }
            }
            return points;
        }

        private List<Point> findBishopMovements(int x, int y, int color)
        {
            List<Point> points = new List<Point>();
            int m = y - 1;
            for (int i = x - 1; i >= 0 & m >= 0; i--, m--) {
                if (chessBoard[m][i] == 0)
                {
                    points.Add(new Point(i, m));
                }
                else
                {
                    if (color != chessBoard[m][i] / 10)
                    {
                        points.Add(new Point(i, m));
                    }
                    break;
                }
            }
            m = y - 1;
            for (int i = x + 1; i <= 7 & m >= 0; i++, m--)
            {
                if (chessBoard[m][i] == 0)
                {
                    points.Add(new Point(i, m));
                }
                else
                {
                    if (color != chessBoard[m][i] / 10)
                    {
                        points.Add(new Point(i, m));
                    }
                    break;
                }
            }
            m = y + 1;
            for (int i = x + 1; i <= 7 & m <= 7; i++, m++)
            {
                if (chessBoard[m][i] == 0)
                {
                    points.Add(new Point(i, m));
                }
                else
                {
                    if (color != chessBoard[m][i] / 10)
                    {
                        points.Add(new Point(i, m));
                    }
                    break;
                }
            }
            m = y + 1;
            for (int i = x - 1; i >= 0 & m <= 7; i--, m++)
            {
                if (chessBoard[m][i] == 0)
                {
                    points.Add(new Point(i, m));
                }
                else
                {
                    if (color != chessBoard[m][i] / 10)
                    {
                        points.Add(new Point(i, m));
                    }
                    break;
                }
            }
            return points;
        }

        private List<Point> findQueenMovements(int x, int y, int color)
        {
            List<Point> points = new List<Point>();
            points.AddRange(findBishopMovements(x, y, color));
            points.AddRange(findRockMovements(x, y, color));
            points.Distinct();
            return points;
        }

        private List<Point> findMovementsWithCombinations(int x, int y, int color, List<Point> combinations)
        {
            List<Point> points = new List<Point>();
            foreach (Point combination in combinations)
            {
                if (y + combination.Y >= 0 & y + combination.Y <= 7 & x + combination.X >= 0 & x + combination.X <= 7)
                {
                    if (chessBoard[y + combination.Y][x + combination.X] / 10 != color || chessBoard[y + combination.Y][x + combination.X] == 0)
                    {
                        points.Add(new Point(x + combination.X, y + combination.Y));
                    }
                }
            }
            return points;
        }

        private void removeCircles()
        {
            foreach (PictureBox circle in circles)
            {
                circle.Dispose();
            }
            circles.Clear();
        }

        private void circleClicked(object sender, EventArgs e)
        {
            PictureBox circle = sender as PictureBox;
            int x = Convert.ToInt32(circle.Name[0].ToString());
            int y = Convert.ToInt32(circle.Name[1].ToString());

            if (pictureBoxes[y][x].Name == "11")
            {
                textBox1.Text = "Crni je pobedio";
                playerTurn = 3;
            }
            else if (pictureBoxes[y][x].Name == "21")
            {
                textBox1.Text = "Beli je pobedio";
                playerTurn = 3;
            }

            pictureBoxes[y][x].ImageLocation = pictureBoxes[LastClick.Y][LastClick.X].ImageLocation;
            pictureBoxes[y][x].Name = pictureBoxes[LastClick.Y][LastClick.X].Name;
            pictureBoxes[LastClick.Y][LastClick.X].ImageLocation = picturesFolderLocation + "0.png";
            pictureBoxes[LastClick.Y][LastClick.X].Name = "0";

            chessBoard[y][x] = chessBoard[LastClick.Y][LastClick.X];
            chessBoard[LastClick.Y][LastClick.X] = 0;
            removeCircles();
            switchTurn();
        }

        private void switchTurn()
        {
            if (playerTurn == 1)
            {
                playerTurn = 2;
            }
            else if (playerTurn == 2)
            {
                playerTurn = 1;
            }
        }

        private void restart_Click(object sender, EventArgs e)
        {
            chessBoard = new List<List<int>>()
        {
            new List<int> {25, 24, 23, 22, 21, 23, 24, 25}, 
            new List<int> {26, 26, 26, 26, 26, 26, 26, 26},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> {16, 16, 16, 16, 16, 16, 16, 16},
            new List<int> {15, 14, 13, 12, 11, 13, 14, 15}
        };
            foreach (List<PictureBox> list in pictureBoxes)
            {
                foreach (PictureBox pictureBox in list)
                {
                    pictureBox.Dispose();
                }
            }
            pictureBoxes.Clear();
            pictureBoxes = generatePictureBoxes();
            playerTurn = 1;
            textBox1.Text = "";
            populateChessBoards();
        }
    }
}
