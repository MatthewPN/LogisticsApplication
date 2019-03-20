using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsApplication
{
    public class Marker
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<string> ConnectedTo { get; set; }
        public Marker(string id, string name, int x, int y) {
            this.ID = id;
            this.Name = name;
            this.X = x;
            this.Y = y;
            this.ConnectedTo = new List<string>();
        }
        public Marker() {
            this.ID = null;
            this.Name = null;
            this.X = -1;
            this.Y = -1;
            this.ConnectedTo = new List<string>();
        }
    }

    public class MarkerContainer
    {
        public List<Marker> Markers { get; set; }

        public MarkerContainer() {
            this.Markers = new List<Marker>();
        }

        public void AddMarker(Marker item) {

            this.Markers.Add(item);
        }
    }

    public class MarkerInstance
    {
        public string MarkerID { get; set; }
        public MarkerInstance(string ID)
        {
            this.MarkerID = ID;
        }
    }

    public class Path
    {
        public List<MarkerInstance> MarkerInstances { get; set; }
        public int Cost { get; set; }
        public Path(string id, int Cost)
        {
            this.Cost = Cost;
            this.MarkerInstances = new List<MarkerInstance>();
            MarkerInstance MI = new MarkerInstance(id);
            this.MarkerInstances.Add(MI);
        }
        public Path(List<MarkerInstance> MIs, int Cost)
        {
            this.Cost = Cost;
            this.MarkerInstances = new List<MarkerInstance>();
            // must create the array list first and then assign each instance object individually.
            // Otherwise, it just makes a pointer reference and does not deep copy the list.
            for (int i=0; i < MIs.Count; i++)
            {
                this.MarkerInstances.Add(MIs[i]);
            }
        }
    }

    public class UniformCostSearch
    {
        public List<Path> Paths { get; set; }
        public UniformCostSearch()
        {
            this.Paths = new List<Path>();
        }
        public UniformCostSearch(string id)
        {
            this.Paths = new List<Path>();
            Path path = new Path(id, 0);
            this.Paths.Add(path);
        }
        public Path RunSearch(MarkerContainer markerContainer, string goalID)
        {
            bool searching = true;
            Path goalPath = new Path("", -1);
            
            //Loop contiously until a goal state
            while (searching)
            {
                // Initialize a null index value
                int expandingPathIndex = -1;
                // Loop through all possible paths within this UniformCostSearch Object
                for (int i=0; i < this.Paths.Count; i++)
                {
                    // if expandingPathIndex is still null
                    if(expandingPathIndex == -1)
                    {
                        // if the path has only unique marker instances within the path
                        if (this.Paths[i].MarkerInstances.Count == this.Paths[i].MarkerInstances.Distinct().Count())
                        {
                            // Set expandingPathIndex as the current iteration
                            expandingPathIndex = i;
                        } else
                        {
                            // Remove this path artifically by increasing its cost value
                            // We could just remove the path through the arraylist, consider this for future refractoring.
                            // Note this path has a cycle and should never be used again as it will have a higher cost than another path
                            //      that does not have a cycle 100% of the time.
                            //this.Paths[i].Cost = 999999;
                            this.Paths.RemoveAt(i);
                            i--;
                        }
                    }
                    else if(this.Paths[expandingPathIndex].Cost > this.Paths[i].Cost)
                    {
                        if(this.Paths[i].MarkerInstances.Count == this.Paths[i].MarkerInstances.Distinct().Count())
                        {
                            expandingPathIndex = i;
                        } else
                        {
                            // Remove this path artifically by increasing its cost value
                            // We could just remove the path through the arraylist, consider this for future refractoring.
                            // Note this path has a cycle and should never be used again as it will have a higher cost than another path
                            //      that does not have a cycle 100% of the time.
                            this.Paths.RemoveAt(i);
                            i--;

                        }
                    }
                }

                if (expandingPathIndex == -1)
                {
                    //no solution that doesn't have a cycle
                    //return null goalpath
                    searching = false;
                    return goalPath;
                }

                if (this.Paths[expandingPathIndex].Cost > goalPath.Cost && goalPath.Cost != -1)
                {
                    searching = false;
                    return goalPath;
                }

                // Write down the last added node within the arraylist (this requires the arraylist keeps insertion order)
                string LastNodeID = this.Paths[expandingPathIndex].MarkerInstances.Last().MarkerID;
                // Create a new marker object
                Marker marker = new Marker();
                for(int i=0; i < markerContainer.Markers.Count; i++)
                {
                    if(markerContainer.Markers[i].ID == LastNodeID)
                    {
                        marker = markerContainer.Markers[i];
                    }
                }

                // For every connectedto ID in the marker
                for (int i =0; i < marker.ConnectedTo.Count; i++)
                {
                    // Create Path Switch as true
                    bool addPath = true;
                    // for every marker instance in the current path
                    for(int j=0; j < this.Paths[expandingPathIndex].MarkerInstances.Count; j++)
                    {
                        // if there is a marker that already contains this ID (hence there would be a cycle)
                        if(this.Paths[expandingPathIndex].MarkerInstances[j].MarkerID == marker.ConnectedTo[i])
                        {
                            // Disable adding this path.
                            addPath = false;
                        }
                    }
                    // If addpath switch is still true
                    if(addPath)
                    {
                        // Create new path with previous path values
                        Path newPath = new Path(this.Paths[expandingPathIndex].MarkerInstances,this.Paths[expandingPathIndex].Cost);
                        MarkerInstance markerInstance = new MarkerInstance(marker.ConnectedTo[i]);
                        // Determine Distance between two points
                        string ParentID = newPath.MarkerInstances.Last().MarkerID;
                        string ChildID = markerInstance.MarkerID;
                        int x1 = Convert.ToInt32(ParentID.Substring(0, 3));
                        int y1 = Convert.ToInt32(ParentID.Substring(3, 3));
                        int x2 = Convert.ToInt32(ChildID.Substring(0, 3));
                        int y2 = Convert.ToInt32(ChildID.Substring(3, 3));
                        double magnitude = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2,2));
                        // Add distance between the two points to the cost
                        newPath.Cost += Convert.ToInt32(magnitude);
                        // Add instance to newPath
                        newPath.MarkerInstances.Add(markerInstance);                        
                        this.Paths.Add(newPath);
                        // If the last added marker has the goal state ID
                        if (marker.ConnectedTo[i] == goalID)
                        {
                            // If the cost of the current goal path is greater than the new path's cost that has a goal state
                            //  OR if goalpath cost is NULL (unassigned)
                            if(goalPath.Cost > newPath.Cost || goalPath.Cost == -1)
                            {
                                // Set the new path as the goal path
                                goalPath = newPath;
                                // initialize lowest cost to a high value for comparisons
                                // This should probably be changed to include a -1 value to represent NULL
                                double lowestCost = 999999;
                                // for every path in the current paths
                                for(int j=0; j < this.Paths.Count; j++)
                                {
                                    // if the cost for the path is lower than the current lowestcost in the iteration
                                    //  AND the path is not the path that we have just expanded
                                    if(this.Paths[j].Cost < lowestCost && this.Paths[expandingPathIndex] != this.Paths[j])
                                    {
                                        // Assign the lowestcost to the path's cost
                                        lowestCost = this.Paths[j].Cost;
                                    }
                                }
                                // If the current goalpath cost is the lowest cost out of all of the paths
                                if(goalPath.Cost <= lowestCost)
                                {
                                    // Stop the loop, there is no better cost for the given paths.
                                    searching = false;
                                    // return the goal path
                                    return goalPath;
                                }
                            }
                        }
                    }
                }
                // Remove this path as it is no longer required
                this.Paths.RemoveAt(expandingPathIndex);
            }

            // No more paths available, returning the currently assigned goalpath
            return goalPath;
        }
    }
}
