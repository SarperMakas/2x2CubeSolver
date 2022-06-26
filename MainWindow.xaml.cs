using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace CubeSolver2x2 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        #region DEFINE VARS
        // Colors of the 
        private Dictionary<string, string> CodeToName = new Dictionary<string, string>() {
            {"#FFFBFDF2", "white"}, {"#FFF9FE6C", "yellow"},
            {"#FFFC0A2D", "red" }, {"#FFFD5E35", "orange"},
            {"#FF00D75A", "green"}, {"#FF0059B9", "blue" }
        };

        private Dictionary<string, string> NameToCode = new Dictionary<string, string>() {
            {"white", "#FFFBFDF2"}, {"yellow", "#FFF9FE6C"},
            {"red", "#FFFC0A2D"}, {"orange", "#FFFD5E35"},
            {"green", "#FF00D75A"}, {"blue" , "#FF0059B9"}
        };

        private Dictionary<int, string> NumberToCode = new Dictionary<int, string>() {
            {0, "#FFFBFDF2"}, {1, "#FFF9FE6C"},
            {2, "#FFFC0A2D"}, {3, "#FFFD5E35"},
            {4, "#FF00D75A"}, {5, "#FF0059B9"}
        };

        private Dictionary<string, int> CodeToNumber = new Dictionary<string, int>() {
            {"#FFFBFDF2", 0}, {"#FFF9FE6C", 1},
            {"#FFFC0A2D", 2}, {"#FFFD5E35", 3},
            {"#FF00D75A", 4}, {"#FF0059B9", 5}
        };

        private Dictionary<string, string> AntiColors = new Dictionary<string, string>() {
            { "orange", "red" }, { "red", "orane" },
            { "white", "yellow" }, { "yellow", "white" },
            { "green", "blue" }, { "blue", "green"}
        };

        // faces of the cubes
        private string[,] backFace;
        private string[,] frontFace;
        private string[,] upFace;
        private string[,] downFace;
        private string[,] rigthFace;
        private string[,] leftFace;

        private string commands = "";

        private Corner corner1, corner2, corner3, corner4, corner5, corner6, corner7, corner8;
        #endregion

        public MainWindow() {
            InitializeComponent();
        }

        private void ChangeColor(object sender, RoutedEventArgs e) {
            Button button = (Button)sender;
            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString((NumberToCode[(CodeToNumber[button.Background.ToString()] + 1) % 6])));
        }

        // It would be much easyer to check corners with arrays
        // cornerN = new string[3] { front/back, rigth/left, up/down };
        private void DefineCorners() {

            // edges of front
            corner1 = new Corner(frontFace[0, 0], leftFace[0, 1], upFace[1, 0]);
            corner2 = new Corner(frontFace[0, 1], rigthFace[0, 0], upFace[1, 1]);
            corner3 = new Corner(frontFace[1, 0], leftFace[1, 1], downFace[0, 0]); //
            corner4 = new Corner(frontFace[1, 1], rigthFace[1 ,0], downFace[0, 1]); //

            // edges of back
            corner5 = new Corner(backFace[1, 0], leftFace[0, 0], upFace[0, 0]);
            corner6 = new Corner(backFace[1, 1], rigthFace[0, 1], upFace[0, 1]);
            corner7 = new Corner(backFace[0, 0], leftFace[1, 0], downFace[1, 0]); //
            corner8 = new Corner(backFace[0, 1], rigthFace[1, 1], downFace[1, 1]); //
        }

        private void SolveCube(object sender, RoutedEventArgs e) {
             DefineFaces();

            if(CheckFirstLayer() == false) {
                AddFirstPart();
                AddOtherDownFaces();
                FixPieces();
            }

            // CheckConditions();
            Ri(); F(); Ri(); B(); B(); R(); Fi(); Ri(); B(); B(); R(); R(); Ui();

            SetColorsOfBlocks();
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(commands)));
        }

        private void SetColorsOfBlocks() {
            // set blocks color according to name code
            BackFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[0, 0]]));
            BackFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[0, 1]]));
            BackFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[1, 0]]));
            BackFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[backFace[1, 1]]));

            UpFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[0, 0]]));
            UpFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[0, 1]]));
            UpFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[1, 0]]));
            UpFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[upFace[1, 1]]));

            FrontFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[0, 0]]));
            FrontFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[0, 1]]));
            FrontFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[1, 0]]));
            FrontFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[frontFace[1, 1]]));

            DownFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[0, 0]]));
            DownFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[0, 1]]));
            DownFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[1, 0]]));
            DownFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[downFace[1, 1]]));

            RigthFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rigthFace[0, 0]]));
            RigthFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rigthFace[0, 1]]));
            RigthFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rigthFace[1, 0]]));
            RigthFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[rigthFace[1, 1]]));

            LeftFaceBlock1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[0, 0]]));
            LeftFaceBlock2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[0, 1]]));
            LeftFaceBlock3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[1, 0]]));
            LeftFaceBlock4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NameToCode[leftFace[1, 1]]));

        }

        private void DefineFaces() {
            backFace = new string[2, 2] {
                { CodeToName[BackFaceBlock1.Background.ToString()], CodeToName[BackFaceBlock2.Background.ToString()]},
                { CodeToName[BackFaceBlock3.Background.ToString()], CodeToName[BackFaceBlock4.Background.ToString()]},
            };
            upFace = new string[2, 2] {
                { CodeToName[UpFaceBlock1.Background.ToString()], CodeToName[UpFaceBlock2.Background.ToString()]},
                { CodeToName[UpFaceBlock3.Background.ToString()], CodeToName[UpFaceBlock4.Background.ToString()]},
            };
            frontFace = new string[2, 2] {
                { CodeToName[FrontFaceBlock1.Background.ToString()], CodeToName[FrontFaceBlock2.Background.ToString()]},
                { CodeToName[FrontFaceBlock3.Background.ToString()], CodeToName[FrontFaceBlock4.Background.ToString()]},
            };
            downFace = new string[2, 2] {
                { CodeToName[DownFaceBlock1.Background.ToString()], CodeToName[DownFaceBlock2.Background.ToString()]},
                { CodeToName[DownFaceBlock3.Background.ToString()], CodeToName[DownFaceBlock4.Background.ToString()]},
            };
            rigthFace = new string[2, 2] {
                { CodeToName[RigthFaceBlock1.Background.ToString()], CodeToName[RigthFaceBlock2.Background.ToString()]},
                { CodeToName[RigthFaceBlock3.Background.ToString()], CodeToName[RigthFaceBlock4.Background.ToString()]},
            };
            leftFace = new string[2, 2] {
                { CodeToName[LeftFaceBlock1.Background.ToString()], CodeToName[LeftFaceBlock2.Background.ToString()]},
                { CodeToName[LeftFaceBlock3.Background.ToString()], CodeToName[LeftFaceBlock4.Background.ToString()]},
            };
        }

        #region RIGTH MOVEMENT
        private void R(bool add = true) {
            
            // first you need to turn the face
            rigthFace = new string[2, 2] {
                {rigthFace[1,0], rigthFace[0,0] },
                {rigthFace[1,1], rigthFace[0,1]}
            };  // remake the face with using the normal face [turn it]

            // get the faces because you need them for turning edges
            string[] backRigth = new string[2] { backFace[0, 1], backFace[1, 1] };
            string[] upRigth = new string[2] { upFace[0, 1], upFace[1, 1] };
            string[] frontRigth = new string[2] { frontFace[0, 1], frontFace[1, 1] };
            string[] downRigth = new string[2] { downFace[0, 1], downFace[1, 1] };

            // Turn Rigth
            backFace[0, 1] = upRigth[0]; backFace[1, 1] = upRigth[1];
            upFace[0, 1] = frontRigth[0]; upFace[1, 1] = frontRigth[1];
            frontFace[0, 1] = downRigth[0]; frontFace[1, 1] = downRigth[1];
            downFace[0, 1] = backRigth[0]; downFace[1, 1] = backRigth[1];

            if (add)
                commands += "R ";
        }

        // Anti rigth
        private void Ri() {
            R(false); R(false); R(false);
            commands += "Ri ";
        }
        #endregion

        #region LEFT MOVEMENT
        private void L(bool add = true) {
            // turn rigthface
            rigthFace = new string[2, 2] {
                {rigthFace[1,0], rigthFace[0,0] },
                {rigthFace[1,1], rigthFace[0,1]}
            };

            string[] backRigth = new string[2] { backFace[0, 1], backFace[1, 1] };
            string[] upRigth = new string[2] { upFace[0, 1], upFace[1, 1] };
            string[] frontRigth = new string[2] { frontFace[0, 1], frontFace[1, 1] };
            string[] downRigth = new string[2] { downFace[0, 1], downFace[1, 1] };

            // Turn Rigth
            backFace[0, 1] = upRigth[0]; backFace[1, 1] = upRigth[1];
            upFace[0, 1] = frontRigth[0]; upFace[1, 1] = frontRigth[1];
            frontFace[0, 1] = downRigth[0]; frontFace[1, 1] = downRigth[1];
            downFace[0, 1] = backRigth[0]; downFace[1, 1] = backRigth[1];

            if (add)
                commands += "Ri ";
        }

        // Anti rigth
        private void Li() {
            L(false); L(false); L(false);
            commands += "Li ";
        }
        #endregion

        #region UP MOVEMENT
        private void U(bool add = true) {
            upFace = new string[2, 2] {
                {upFace[1,0], upFace[0,0] },
                {upFace[1,1], upFace[0,1]}
            };

            string[] backUp = new string[2] { backFace[1, 0], backFace[1, 1] };
            string[] frontUp = new string[2] { frontFace[0, 0], frontFace[0, 1] };
            string[] rigthUp = new string[2] { rigthFace[0, 0], rigthFace[0, 1] };
            string[] leftUp = new string[2] { leftFace[0, 0], leftFace[0, 1] };

            backFace[1, 0] = leftUp[1]; backFace[1, 1] = leftUp[0];
            frontFace[0, 0] = rigthUp[0]; frontFace[0, 1] = rigthUp[1];
            rigthFace[0, 0] = backUp[1]; rigthFace[0, 1] = backUp[0];
            leftFace[0, 0] = frontUp[0]; leftFace[0, 1] = frontUp[1];

            if (add)
                commands += "U ";
        }

        private void Ui() {
            U(false); U(false); U(false);
            commands += "Ui ";
        }
        #endregion

        #region FRONT MOVEMENT
        private void F(bool add = true) {
            frontFace = new string[2, 2] {
                { frontFace[1,0], frontFace[0,0] },
                { frontFace[1,1], frontFace[0,1] }
            };

            string[] rigthFront = new string[2] { rigthFace[0, 0], rigthFace[1, 0] };
            string[] downFront = new string[2] { downFace[0, 0], downFace[0, 1] };
            string[] leftFront = new string[2] { leftFace[0, 1], leftFace[1, 1] };
            string[] upFront = new string[2] { upFace[1, 0], upFace[1, 1] };

            upFace[1, 0] = leftFront[1]; upFace[1, 1] = leftFront[0];
            leftFace[0, 1] = downFront[0]; leftFace[1, 1] = downFront[1];
            downFace[0, 0] = rigthFront[1]; downFace[0, 1] = rigthFront[0];
            rigthFace[0, 0] = upFront[0]; rigthFace[1, 0] = upFront[1];

            if (add)
                commands += "F ";
        }

        private void Fi() {
            F(false); F(false); F(false);
            commands += "Fi ";
        }
        #endregion

        #region BACK MOVEMENT
        private void B(bool add = true) {

            // turn back face
            backFace = new string[2, 2] {
                { backFace[1,0], backFace[0,0] },
                { backFace[1,1], backFace[0,1] }
            };

            // turn back
            string[] upBack = new string[2] { upFace[0,0], upFace[0,1] };
            string[] rigthBack = new string[2] { rigthFace[0,1], rigthFace[1,1] };
            string[] downBack = new string[2] { downFace[1, 0], downFace[1, 1] };
            string[] leftBack = new string[2] { leftFace[0, 0], leftFace[1, 0] };

            upFace[0, 0] = rigthBack[0]; upFace[0, 1] = rigthBack[1];
            rigthFace[0, 1] = downBack[1]; rigthFace[1, 1] = downBack[0];
            downFace[1, 0] = leftBack[0]; downFace[1, 1] = leftBack[1];
            leftFace[0, 0] = upBack[1]; leftFace[1, 0] = upBack[0];

            if (add)
                commands += "B ";
        }

        
        private void Bi() {
            B(false); B(false); B(false);
            commands += "Bi ";
        }
        #endregion

        #region DOWN MOVEMENT
        private void D(bool add = true) {

            // turn face face
            downFace = new string[2, 2] {
                { downFace[1,0], downFace[0,0] },
                { downFace[1,1], downFace[0,1] }
            };

            // turn face
            string[] backDown = new string[2] { backFace[0, 0], backFace[0, 1] };
            string[] rigthDown = new string[2] { rigthFace[1, 0], rigthFace[1, 1] };
            string[] leftDown = new string[2] { leftFace[1, 0], leftFace[1, 1] };
            string[] frontDown = new string[2] { frontFace[1, 0], frontFace[1, 1] };

            frontFace[1, 0] = leftDown[0]; frontFace[1, 1] = leftDown[1];
            rigthFace[1, 0] = frontDown[0]; rigthFace[1, 1] = frontDown[1];
            backFace[0, 0] = rigthDown[1]; backFace[0, 1] = rigthDown[0];
            leftFace[1, 0] = backDown[1]; leftFace[1, 1] = backDown[0];

            if (add)
                commands += "D ";

        }

        private void Di() {
            D(false); D(false); D(false);
            commands += "Di ";
        }
        #endregion

        #region FIRST LAYER
        private bool CheckFirstLayer() { 
            // First if the first layer of the cube is done or not done wish that it is done
            if (downFace[0, 0] == downFace[0, 1] && downFace[0, 1] == downFace[1, 0] && downFace[1, 0] == downFace[1, 1] && downFace[1, 1] == "white") {
                // checkk edges
                if (rigthFace[1, 0] == rigthFace[1, 1] && leftFace[1, 0] == leftFace[1, 1] && frontFace[1, 0] == frontFace[1, 1] && backFace[0,0] == backFace[0, 1]) {
                    // then the first layer is done
                    return true;
                }
            }
            // Not? then return false
            return false;
        }

        // Now we need to  solve the first layer at the down part
        // firstly putting a only piece to the layer then put athor pieces to that layer
        private void AddFirstPart() {
            DefineCorners();


            // check if there is a white piece 
            if (downFace[0,0] == "white" || downFace[0, 1] == "white" || downFace[1, 0] == "white" || downFace[1, 1] == "white") {
                // put a white to downFace[0,0]
                while (downFace[0, 0] != "white")
                    D();
            // put the piece to correct part
            } else if (corner2.elements.Contains("white")) {
                Ri(); Di();
            } else if (corner1.elements.Contains("white")) {
                L();
            } else if (corner5.elements.Contains("white")) {
                L(); L();
            } else if (corner6.elements.Contains("white")) {
                R(); D(); D();
            }
            DefineCorners();

            Trace.WriteLine("Adding White To downFace[0, 0] DONE\n");
        }

        private void AddOtherDownFaces() {
            DefineCorners();

            
            Trace.WriteLine(!(corner3.elements.Contains("white") && corner4.elements.Contains("white") && corner7.elements.Contains("white") && corner8.elements.Contains("white")));
            while (!(corner3.elements.Contains("white") && corner4.elements.Contains("white") && corner7.elements.Contains("white") && corner8.elements.Contains("white"))) {
                // while not all the elements of the down layer isn't contains white so we need to make them all white
                // there are 2 posibilitys the white parts can be in the up or down layer we want them to be in down layer

                Trace.WriteLine("Start Checking Corner1");
                // first we need to put the white piece to corner 1
                while (!corner1.elements.Contains("white")) {
                    U();
                    Trace.WriteLine($"{corner1.rl} {corner1.fb} {corner1.ud}");
                    DefineCorners();
                }

                Trace.WriteLine("Start Checking Corner4");
                // after that we need to put the white pieces to the down so we need empty space
                while (corner4.elements.Contains("white")) {
                    D();
                    DefineCorners();
                }

                // now we have a empty space and a piece now we can put it
                R(); Ui(); Ri();
                DefineCorners();
            }

            // Put 3 white piece to Up because we need to put them to correct places
            Bi(); R(); Bi();

            Ui(); D(); // turn cube

            // fix it
            while (downFace[0, 1] != "white") {
                R(); U(); Ri(); Ui(); R(); U(); Ri();
            }
            U(); Di(); //turn again

            Trace.WriteLine("Adding White pices to the Up DONE\n");
        }

        private void FixPieces() {
            DefineCorners();
            Trace.WriteLine(downFace[0, 0] != "white" || downFace[1, 0] != "white" || downFace[0, 1] != "white" || downFace[1, 1] != "white");
            while (downFace[0, 0] != "white" || downFace[1, 0] != "white" || downFace[0, 1] != "white" || downFace[1, 1] != "white") {

                DefineCorners();

                // find correct piece
                Trace.WriteLine("find correct piece");
                Trace.WriteLine($"{!(corner1.elements.Contains("white") && corner1.elements.Contains(corner3.fb))} {corner1.elements.Contains("white")}  {corner1.elements.Contains(corner3.fb)}");
                while (!(corner1.elements.Contains("white") && corner1.elements.Contains(corner3.fb))) {
                    U();
                    DefineCorners();
                }

                // now put the piece correctly
                Trace.WriteLine("now put the piece correctly");
                while (downFace[0, 1] != "white") {
                    Ui(); R(); U(); Ri();
                }

                Di();
                Trace.WriteLine(commands);
            }
        }
        #endregion

        #region SECOND LAYER
        // fistly you need to know how to solve 2d cube
        // there are some uniq conditiuons
        private void CheckConditions() {
            string cc = AntiColors[downFace[0, 0]]; // checked color
            if (upFace[0, 0] == cc && upFace[0, 1] == cc && upFace[1, 0] == cc && upFace[1, 1] == cc) {
                DoneBack(); // break if all up face is yellow
            } else {
                while (!(upFace[0, 0] == cc && upFace[0, 1] == cc && upFace[1, 0] == cc && upFace[1, 1] == cc)) {

                    for (int i = 0; i < 4; i++) {
                        // now we are making all of the up face cc color

                        if ((upFace[0, 1] == upFace[0, 0] && upFace[0, 0] == frontFace[0, 0] && frontFace[0, 0] == frontFace[0, 1] && frontFace[0, 1] == cc) ||
                            (upFace[0, 1] == upFace[1, 0] && upFace[0, 1] == cc) || upFace[1, 0] == cc) {
                            R(); U(); Ri(); U(); R(); U(); U(); Ri();
                        }

                        if (upFace[0, 0] == cc && upFace[0, 1] == cc && upFace[1, 0] == cc && upFace[1, 1] == cc)
                            break; // break if all up face is yellow

                        U(); // first we need to check it
                    }
                }
                DoneBack();
            }
        }

        private void DoneBack() {

            // get wich side is done
            bool isRigth = (rigthFace[0, 0] == rigthFace[0, 1]);
            bool isFront = (frontFace[0, 0] == frontFace[0, 1]);
            bool isLeft = (leftFace[0, 0] == leftFace[0, 1]);
            bool isBack = (backFace[1, 0] == backFace[1, 1]);

            if (isRigth == true) {
                while (rigthFace[1, 0] != rigthFace[0, 0])
                    D();
                D(); Ui();
            } else if(isLeft == true) {
                while (leftFace[1, 0] != leftFace[0, 0])
                    D();
                Di(); U();
            } else if (isFront == true) {
                while (frontFace[0, 0] != frontFace[1, 0])
                    D();
                D(); D(); U(); U();
            } else if (isBack == true) {
                while (backFace[0, 0] != backFace[1, 0])
                    D();
            }

        }

        #endregion
    }

    public class Corner {

        public string fb, rl, ud;
        public string[] elements;

        public Corner(string frontBack, string rigthLeft, string upDown) {
            this.fb = frontBack; this.rl = rigthLeft; this.ud = upDown;
            elements = new string[3] { fb, rl, ud};
        }
    }
}
