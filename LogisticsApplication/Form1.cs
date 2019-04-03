using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LogisticsApplication
{
    public partial class Form1 : Form
    {
        public MarkerContainer PointContainers = new MarkerContainer();
        public bool AddNewMarker = false;
        public bool AddNewConnector = false;
        public bool DraggingNewConnector = false;
        public bool RunLogistics = false;
        public bool chosenLogisticsStart = false;
        public Point ConnectorPoint1;
        public Marker theChosenMarker;
        public int[] theChosenMarkerIndexes = new int[2] { -1, -1 };

        public Form1()
        {
            InitializeComponent();
            using (StreamReader r = new StreamReader("data.json"))
            {
                string json = r.ReadToEnd();
                PointContainers = JsonConvert.DeserializeObject<MarkerContainer>(json);
            }
        }

        private void LogisticsRun(string startID, string endID) {
            // Create a uniformCostSearch object
            UniformCostSearch uniformCostSearch = new UniformCostSearch(startID);
            UniformCostSearch uniformHeuristicCostSearch = new UniformCostSearch(startID);
            // Use the Runsearch function of the uniform cost search class to get the goal path
            Path GoalPath = uniformCostSearch.RunSearch(PointContainers, endID);
            Path HueristicGoalPath = uniformHeuristicCostSearch.RunHeuristicSearch(PointContainers, endID);
            // For every marker instance in the goal path (except the first iteration)
            for (int i=1; i < GoalPath.MarkerInstances.Count; i++)
            {
                // Get the X,Y coordinates of the points for drawing them.
                int x1 = Convert.ToInt32(GoalPath.MarkerInstances[i-1].MarkerID.Substring(0,3));
                int y1 = Convert.ToInt32(GoalPath.MarkerInstances[i-1].MarkerID.Substring(3, 3));
                int x2 = Convert.ToInt32(GoalPath.MarkerInstances[i].MarkerID.Substring(0, 3));
                int y2 = Convert.ToInt32(GoalPath.MarkerInstances[i].MarkerID.Substring(3, 3));

                // Only draw this marker for the first iteration
                if ( i == 1 ) { Draw_Marker(x1, y1, Color.Green); }
                // Draw the marker at the X2,Y2 coordinate
                Draw_Marker(x2,y2,Color.Green);
                // Draw the line between two different markers
                Draw_Line(x1,y1,x2,y2,Color.Green);
            }

            // This is for debugging, it shows the output of the goalpath in JSON formatting
            string output = "Standard: " + JsonConvert.SerializeObject(GoalPath);
            Console.WriteLine(output);
            string output2 = "Hueristic: " + JsonConvert.SerializeObject(HueristicGoalPath);
            Console.WriteLine(output2);
            if (GoalPath.Cost != -1) {
                // Convert cost to total miles
                double cost = GoalPath.Cost;
                // Every 85 magnitude is equal to 1,000 FT
                cost = cost / 85 * 1000;
                // 5280 feet in every mile
                cost = cost / 5280;
                this.label2.Text = "Miles: " + cost.ToString();
                this.label1.Text = "Expansions: " + GoalPath.TotalExpansions.ToString();
                this.label3.Text = "Heuristic Expansions: " + HueristicGoalPath.TotalExpansions.ToString();
                Console.WriteLine("The shortest path has a distance of '" + cost.ToString() + "' miles");
            } else
            {
                Console.WriteLine("There is no possible path to reach that destination.");
            }
            // Return void
            return;
        }

        private void Picture_Click(object sender, System.EventArgs e)
        {
            if (AddNewMarker == true)
            {
                // Disable adding anymore markers
                // AddNewMarker = false; // Commented out so that you can add many markers
                MouseEventArgs mouseEventArgs = e as MouseEventArgs;
                if (mouseEventArgs != null)
                {
                    Create_Marker(mouseEventArgs);
                }
            }
            else if(AddNewConnector == true) {
                MouseEventArgs mouseEventArgs = e as MouseEventArgs;
                Marker candidate = new Marker();
                double smallestLength=0;
                for (int i = 0; i < PointContainers.Markers.Count; i++)
                {
                    if (candidate.ID == null)
                    {
                        int xDiff = mouseEventArgs.X - PointContainers.Markers[i].X;
                        int yDiff = mouseEventArgs.Y - PointContainers.Markers[i].Y;
                        double length = Math.Sqrt(Math.Pow(xDiff,2)+Math.Pow(yDiff,2));
                        if (length < 15/2)
                        {
                            candidate = PointContainers.Markers[i];
                            smallestLength = length;
                            theChosenMarkerIndexes[0] = i;
                        }
                    }
                    else {
                        int xDiff = mouseEventArgs.X - PointContainers.Markers[i].X;
                        int yDiff = mouseEventArgs.Y - PointContainers.Markers[i].Y;
                        double length = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                        if (length < smallestLength) {
                            candidate = PointContainers.Markers[i];
                            smallestLength = length;
                            theChosenMarkerIndexes[0] = i;
                        }
                    }

                }

                if (candidate.ID != null)
                {
                    theChosenMarker = candidate;
                }
                else {
                    return;
                }

                AddNewConnector = false; // Comment out to add many connectors
                DraggingNewConnector = true;
                ConnectorPoint1 = new Point(mouseEventArgs.X, mouseEventArgs.Y);
            }
            else if(RunLogistics == true)
            {
                MouseEventArgs mouseEventArgs = e as MouseEventArgs;
                Marker candidate = new Marker();
                double smallestLength = 0;
                for (int i = 0; i < PointContainers.Markers.Count; i++)
                {
                    if (candidate.ID == null)
                    {
                        int xDiff = mouseEventArgs.X - PointContainers.Markers[i].X;
                        int yDiff = mouseEventArgs.Y - PointContainers.Markers[i].Y;
                        double length = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                        if (length < 15 / 2)
                        {
                            candidate = PointContainers.Markers[i];
                            smallestLength = length;
                            theChosenMarkerIndexes[0] = i;
                        }
                    }
                    else
                    {
                        int xDiff = mouseEventArgs.X - PointContainers.Markers[i].X;
                        int yDiff = mouseEventArgs.Y - PointContainers.Markers[i].Y;
                        double length = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                        if (length < smallestLength)
                        {
                            candidate = PointContainers.Markers[i];
                            smallestLength = length;
                            theChosenMarkerIndexes[0] = i;
                        }
                    }
                }

                if (candidate.ID != null)
                {
                    theChosenMarker = candidate;
                }
                else
                {
                    return;
                }

                RunLogistics = false;
                chosenLogisticsStart = true;
                Draw_Marker(candidate.X, candidate.Y, Color.Yellow);
            }
            else if(chosenLogisticsStart == true)
            {
                MouseEventArgs mouseEventArgs = e as MouseEventArgs;
                Marker candidate = new Marker();
                double smallestLength = 0;
                for (int i = 0; i < PointContainers.Markers.Count; i++)
                {
                    if (candidate.ID == null)
                    {
                        int xDiff = mouseEventArgs.X - PointContainers.Markers[i].X;
                        int yDiff = mouseEventArgs.Y - PointContainers.Markers[i].Y;
                        double length = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                        if (length < 15 / 2)
                        {
                            candidate = PointContainers.Markers[i];
                            smallestLength = length;
                        }
                    }
                    else
                    {
                        int xDiff = mouseEventArgs.X - PointContainers.Markers[i].X;
                        int yDiff = mouseEventArgs.Y - PointContainers.Markers[i].Y;
                        double length = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                        if (length < smallestLength)
                        {
                            candidate = PointContainers.Markers[i];
                            smallestLength = length;
                        }
                    }
                }

                if (candidate.ID != null)
                {
                }
                else
                {
                    return;
                }

                chosenLogisticsStart = false;
                Draw_Marker(candidate.X, candidate.Y, Color.Yellow);
                LogisticsRun(theChosenMarker.ID, candidate.ID);
            }
        }

        private void Picture_Release(object sender, System.EventArgs e) {
            if (DraggingNewConnector == false) {
                return;
            }


            MouseEventArgs mouseEventArgs = e as MouseEventArgs;
            Marker candidate = new Marker();
            double smallestLength = 0;
            for (int i = 0; i < PointContainers.Markers.Count; i++)
            {
                if (candidate.ID == null)
                {
                    int xDiff = mouseEventArgs.X - PointContainers.Markers[i].X;
                    int yDiff = mouseEventArgs.Y - PointContainers.Markers[i].Y;
                    double length = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                    if (length < 15 / 2)
                    {
                        candidate = PointContainers.Markers[i];
                        smallestLength = length;
                        theChosenMarkerIndexes[1] = i;
                        Console.WriteLine("Adding New Candidate");   
                    }
                }
                else
                {
                    int xDiff = mouseEventArgs.X - PointContainers.Markers[i].X;
                    int yDiff = mouseEventArgs.Y - PointContainers.Markers[i].Y;
                    double length = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                    if (length < smallestLength)
                    {
                        candidate = PointContainers.Markers[i];
                        smallestLength = length;
                        theChosenMarkerIndexes[1] = i;
                        Console.WriteLine("Overwriting Candidate");
                    }
                }
            }

            if (candidate.ID == null)
            {
                Console.WriteLine("Candidate is NULL");
                return;
            }

            AddNewConnector = true; // Comment out to not add many connectors at a time
            DraggingNewConnector = false;
            //bool addNewConnection = true;
            for (int i = 0; i < PointContainers.Markers[theChosenMarkerIndexes[0]].ConnectedTo.Count; i++) {
                if (PointContainers.Markers[theChosenMarkerIndexes[0]].ConnectedTo[i] == PointContainers.Markers[theChosenMarkerIndexes[1]].ID) {
                    //addNewConnection = false;
                    return;
                }
            }

            Point p1 = new Point();
            Point p2 = new Point();

            p1.X = PointContainers.Markers[theChosenMarkerIndexes[0]].X;
            p1.Y = PointContainers.Markers[theChosenMarkerIndexes[0]].Y;
            p2.X = PointContainers.Markers[theChosenMarkerIndexes[1]].X;
            p2.Y = PointContainers.Markers[theChosenMarkerIndexes[1]].Y;
            Point difVector = new Point();
            difVector.X = p1.X - p2.X;
            difVector.Y = p1.Y - p2.Y;

            if(difVector.X != 0 && difVector.Y != 0) {
                double magnitude = Math.Sqrt(Math.Pow(difVector.X, 2) + Math.Pow(difVector.Y, 2));
                double unitX = difVector.X / magnitude;
                double unitY = difVector.Y / magnitude;
                Point sP1 = new Point();
                Point sP2 = new Point();
                sP1.X = Convert.ToInt32(p1.X - unitX * (15 / 2));
                sP1.Y = Convert.ToInt32(p1.Y - unitY * (15 / 2));
                sP2.X = Convert.ToInt32(p2.X + unitX * (15 / 2));
                sP2.Y = Convert.ToInt32(p2.Y + unitY * (15 / 2));

                PointContainers.Markers[theChosenMarkerIndexes[0]].ConnectedTo.Add(PointContainers.Markers[theChosenMarkerIndexes[1]].ID);
                PointContainers.Markers[theChosenMarkerIndexes[1]].ConnectedTo.Add(PointContainers.Markers[theChosenMarkerIndexes[0]].ID);

                Draw_Line(sP1.X, sP1.Y, sP2.X, sP2.Y,Color.Red);
            }
        }

        private void Picture_Move(object sender, System.EventArgs e) {
            if (DraggingNewConnector == false) {
                return;
            }
            return;
            MouseEventArgs mouseEventArgs = e as MouseEventArgs;
            Draw_Line(ConnectorPoint1.X, ConnectorPoint1.Y, mouseEventArgs.X, mouseEventArgs.Y, Color.Red);
        }

        private void Create_Marker(MouseEventArgs e) {

            string x = e.X.ToString();
            string y = e.Y.ToString();

            for (int i = 0; i < (3 - x.Length);)
            {
                x = "0" + x;
            }

            for (int i = 0; i < (3 - y.Length);)
            {
                y = "0" + y;
            }
            // Create instance of Marker class with constructor
            Marker markitem = new Marker(x + y, "", e.X, e.Y);
            // Add instance of Marker class to the List<Marker> object in PointContainers object
            PointContainers.AddMarker(markitem);
            // Output JSON of markitem Marker object
            string output = JsonConvert.SerializeObject(markitem);
            Console.WriteLine(output);
            // Output JSON of PointContainers List<Marker> 
            output = JsonConvert.SerializeObject(PointContainers);
            Console.WriteLine(output);
            // Draw Marker on screen at clicked location
            Draw_Marker(e.X, e.Y, Color.Red);
        }

        private void Draw_Marker(int x, int y, Color color) {
            Graphics g = this.pictureBox1.CreateGraphics();
            int height = 15;
            int width = 15;

            g.DrawEllipse(
                new Pen(color, 2f),
                x-(width/2), y - (height / 2),
                width, height);
        }

        private void Draw_Line(int x1, int y1, int x2, int y2, Color color) {
            Graphics g = this.pictureBox1.CreateGraphics();
            g.DrawLine(
                new Pen(color, 2f),
                new Point(x1, y1),
                new Point(x2, y2)
                );
        }

        private void newMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewMarker = true;
            AddNewConnector = false;
            DraggingNewConnector = false;
            RunLogistics = false;
            chosenLogisticsStart = false;

            return;
        }

        private void newMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewMarker = true;
            AddNewConnector = false;
            DraggingNewConnector = false;
            RunLogistics = false;
            chosenLogisticsStart = false;
        }

        private void connectMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewMarker = false;
            AddNewConnector = true;
            DraggingNewConnector = false;
            RunLogistics = false;
            chosenLogisticsStart = false;
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            Draw_AllMarkers();
        }

        private void Draw_AllMarkers() {
            for (int i=0; i < PointContainers.Markers.Count; i++) {
                Point mP1 = new Point(PointContainers.Markers[i].X, PointContainers.Markers[i].Y);
                Draw_Marker(mP1.X, mP1.Y, Color.Red);
                for (int j = 0; j < PointContainers.Markers[i].ConnectedTo.Count; j++) {
                    Point p1 = new Point();
                    Point p2 = new Point();

                    p1.X = PointContainers.Markers[i].X;
                    p1.Y = PointContainers.Markers[i].Y;
                    p2.X = Convert.ToInt32(PointContainers.Markers[i].ConnectedTo[j].Substring(0,3));
                    p2.Y = Convert.ToInt32(PointContainers.Markers[i].ConnectedTo[j].Substring(3,3));
                    Point difVector = new Point();
                    difVector.X = p1.X - p2.X;
                    difVector.Y = p1.Y - p2.Y;
                    double magnitude = Math.Sqrt(Math.Pow(difVector.X, 2) + Math.Pow(difVector.Y, 2));
                    double unitX = difVector.X / magnitude;
                    double unitY = difVector.Y / magnitude;
                    Point sP1 = new Point();
                    Point sP2 = new Point();
                    sP1.X = Convert.ToInt32(p1.X - unitX * (15 / 2));
                    sP1.Y = Convert.ToInt32(p1.Y - unitY * (15 / 2));
                    sP2.X = Convert.ToInt32(p2.X + unitX * (15 / 2));
                    sP2.Y = Convert.ToInt32(p2.Y + unitY * (15 / 2));

                    Draw_Line(sP1.X, sP1.Y, sP2.X, sP2.Y, Color.Red);
                }
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void runLogisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewMarker = false;
            AddNewConnector = false;
            DraggingNewConnector = false;
            RunLogistics = true;
            chosenLogisticsStart = false;
            Draw_AllMarkers();
        }
    }
}
