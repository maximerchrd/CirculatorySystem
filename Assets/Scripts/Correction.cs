using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

static public class Correction
{
    static public bool CorrectionLevelA(Tilemap tilemap, List<List<Vector3Int>> arteriesLines, string[] organNames)
    {
        Vector3Int heartPosition = new Vector3Int();
        Dictionary<Vector3Int, List<Vector3Int>> arteriesConnections = BuildConnectionsMap(arteriesLines);
        
        
        foreach (var entry in arteriesConnections)
        {
            if (tilemap.GetTile(entry.Key).name == "heart")
            {
                heartPosition = entry.Key;
                if (entry.Value.Count != 1)
                {
                    return false;
                }
            }
            else if (tilemap.GetTile(entry.Key).name == "lungs")
            {
                heartPosition = entry.Key;
                if (entry.Value.Count != 1)
                {
                    return false;
                }
            }
            else
            {
                if (entry.Value.Count != 2)
                {
                    return false;
                }
            }
        }
        
        Debug.Log("Number of connections per Tile is correct");
        
        //check connections between organs
        for (int k = 0; k < organNames.Length; k++)
        {
            Vector3Int organ = FindUniqueTileWithName(organNames[k], arteriesConnections, tilemap);
            List<List<Vector3Int>> arteriesPathsToOrgan = new List<List<Vector3Int>>();
            arteriesPathsToOrgan = FindPathsToHeart(arteriesPathsToOrgan, organ, heartPosition, arteriesConnections, tilemap);
            for (int i = arteriesPathsToOrgan.Count - 1; i >= 0; i--)
            {
                if (arteriesPathsToOrgan[i][arteriesPathsToOrgan[i].Count - 1] != heartPosition)
                {
                    arteriesPathsToOrgan.Remove(arteriesPathsToOrgan[i]);
                }
            }

            if (arteriesPathsToOrgan.Count != 1)
            {
                Debug.Log("artery for: " + organNames[k] + " is wrong. Not right number: " + arteriesPathsToOrgan.Count);
                PrintPaths(arteriesPathsToOrgan);
               
                return false;
            }
            
            Debug.Log("artery for: " + organNames[k] + " is correct");
        }
        
        return true;
    }
    
    static public bool CorrectionLevelB(Tilemap tilemap, List<List<Vector3Int>> arteriesLines, List<List<Vector3Int>> veinsLines,
        string[] organNames)
    {
        Vector3Int heartPosition = new Vector3Int();
        Dictionary<Vector3Int, List<Vector3Int>> arteriesConnections = BuildConnectionsMap(arteriesLines);
        Dictionary<Vector3Int, List<Vector3Int>> veinsConnections = BuildConnectionsMap(veinsLines);
        
        //check number of connections for each tile
        Dictionary<Vector3Int, List<Vector3Int>> allConnections =
            MergeConnections(arteriesConnections, veinsConnections);
        
        foreach (var entry in allConnections)
        {
            if (tilemap.GetTile(entry.Key).name == "heart")
            {
                heartPosition = entry.Key;
                if (entry.Value.Count != 2)
                {
                    return false;
                }
            }
            else
            {
                if (entry.Value.Count != 2)
                {
                    return false;
                }
            }
        }
        
        Debug.Log("Number of connections per Tile is correct");
        
        //check connections between organs
        for (int k = 0; k < organNames.Length; k++)
        {
            Vector3Int organ = FindUniqueTileWithName(organNames[k], allConnections, tilemap);
            List<List<Vector3Int>> arteriesPathsToOrgan = new List<List<Vector3Int>>();
            arteriesPathsToOrgan = FindPathsToHeart(arteriesPathsToOrgan, organ, heartPosition, arteriesConnections, tilemap);
            for (int i = arteriesPathsToOrgan.Count - 1; i >= 0; i--)
            {
                if (arteriesPathsToOrgan[i][arteriesPathsToOrgan[i].Count - 1] != heartPosition)
                {
                    arteriesPathsToOrgan.Remove(arteriesPathsToOrgan[i]);
                }
            }

            if (arteriesPathsToOrgan.Count != 1)
            {
                Debug.Log("artery for: " + organNames[k] + " is wrong. Not right number: " + arteriesPathsToOrgan.Count);
                PrintPaths(arteriesPathsToOrgan);
               
                return false;
            }
            
            Debug.Log("artery for: " + organNames[k] + " is correct");
        
            List<List<Vector3Int>> veinsPathsToOrgan = new List<List<Vector3Int>>();
            veinsPathsToOrgan = FindPathsToHeart(veinsPathsToOrgan, organ, heartPosition, veinsConnections, tilemap);
            for (int i = veinsPathsToOrgan.Count - 1; i >= 0; i--)
            {
                if (veinsPathsToOrgan[i][veinsPathsToOrgan[i].Count - 1] != heartPosition)
                {
                    veinsPathsToOrgan.Remove(veinsPathsToOrgan[i]);
                }
            }
            
            if (veinsPathsToOrgan.Count != 1)
            {
                return false;
            }
            
            Debug.Log("vein for: " + organNames[k] + " is correct");
        }
        
        return true;
    }
    
    static public bool CorrectionLevelC(Tilemap tilemap, List<List<Vector3Int>> arteriesLines, List<List<Vector3Int>> veinsLines,
        string[] organNames)
    {
        Vector3Int heartPosition = new Vector3Int();
        Dictionary<Vector3Int, List<Vector3Int>> arteriesConnections = BuildConnectionsMap(arteriesLines);
        Dictionary<Vector3Int, List<Vector3Int>> veinsConnections = BuildConnectionsMap(veinsLines);
        
        //check number of connections for each tile
        Dictionary<Vector3Int, List<Vector3Int>> allConnections =
            MergeConnections(arteriesConnections, veinsConnections);
        
        foreach (var entry in allConnections)
        {
            if (tilemap.GetTile(entry.Key).name == "heart")
            {
                heartPosition = entry.Key;
                if (organNames.Length > 1 && entry.Value.Count != 4 || 
                    organNames.Length == 1 && entry.Value.Count != 2)
                {
                    return false;
                }
            }
            else
            {
                if (entry.Value.Count != 2)
                {
                    return false;
                }
            }
        }
        
        Debug.Log("Number of connections per Tile is correct");
        
        //check connections between organs
        for (int k = 0; k < organNames.Length; k++)
        {
            Vector3Int organ = FindUniqueTileWithName(organNames[k], allConnections, tilemap);
            List<List<Vector3Int>> arteriesPathsToOrgan = new List<List<Vector3Int>>();
            arteriesPathsToOrgan = FindPathsToHeart(arteriesPathsToOrgan, organ, heartPosition, arteriesConnections, tilemap);
            for (int i = arteriesPathsToOrgan.Count - 1; i >= 0; i--)
            {
                if (arteriesPathsToOrgan[i][arteriesPathsToOrgan[i].Count - 1] != heartPosition)
                {
                    arteriesPathsToOrgan.Remove(arteriesPathsToOrgan[i]);
                }
            }

            if (arteriesPathsToOrgan.Count != 1)
            {
                Debug.Log("artery for: " + organNames[k] + " is wrong. Not right number: " + arteriesPathsToOrgan.Count);
                PrintPaths(arteriesPathsToOrgan);
               
                return false;
            }
            
       
            List<Vector3Int> path = DeterminePathDirection(arteriesPathsToOrgan[0], tilemap);
            //arteries flow from heart to organs
            if (path.Count > 0 && path[0] != heartPosition)
            {
                Debug.Log("artery for: " + organNames[k] + " is wrong. Not right direction");
                return false;
            }
            
            Debug.Log("artery for: " + organNames[k] + " is correct");
        
            List<List<Vector3Int>> veinsPathsToOrgan = new List<List<Vector3Int>>();
            veinsPathsToOrgan = FindPathsToHeart(veinsPathsToOrgan, organ, heartPosition, veinsConnections, tilemap);
            for (int i = veinsPathsToOrgan.Count - 1; i >= 0; i--)
            {
                if (veinsPathsToOrgan[i][veinsPathsToOrgan[i].Count - 1] != heartPosition)
                {
                    veinsPathsToOrgan.Remove(veinsPathsToOrgan[i]);
                }
            }
            
            if (veinsPathsToOrgan.Count != 1)
            {
                return false;
            }
       
            List<Vector3Int> veinPath = DeterminePathDirection(veinsPathsToOrgan[0], tilemap);
            //veins flow from organs to heart
            if (veinPath[veinPath.Count - 1] != heartPosition)
            {
                return false;
            }
            
            Debug.Log("vein for: " + organNames[k] + " is correct");
        }
        
        return true;
    }

    static private List<Vector3Int> DeterminePathDirection(List<Vector3Int> path, Tilemap tilemap)
    {
        for (int i = 1; i < path.Count; i++)
        {
            if (tilemap.GetTile(path[i]).name.Contains("fromleft"))
            {
                if (path[i+1] == path[i] + Vector3Int.right)
                {
                    path.Reverse();
                    return path;
                } 
                if (path[i-1] == path[i] + Vector3Int.right)
                {
                    return path;
                }
            } else if (tilemap.GetTile(path[i]).name.Contains("fromright"))
            {
                if (path[i+1] == path[i] + Vector3Int.right)
                {
                    path.Reverse();
                    return path;
                } 
                if (path[i-1] == path[i] + Vector3Int.right)
                {
                    return path;
                }
            } else if (tilemap.GetTile(path[i]).name.Contains("fromupleft"))
            {
                if (path[i+1] == path[i] + Vector3Int.left + Vector3Int.up)
                {
                    path.Reverse();
                    return path;
                } 
                if (path[i-1] == path[i] + Vector3Int.left + Vector3Int.up)
                {
                    return path;
                }
            } else if (tilemap.GetTile(path[i]).name.Contains("fromupright"))
            {
                if (path[i+1] == path[i] + Vector3Int.right + Vector3Int.up)
                {
                    path.Reverse();
                    return path;
                } 
                if (path[i-1] == path[i] + Vector3Int.right + Vector3Int.up)
                {
                    return path;
                }
            } else if (tilemap.GetTile(path[i]).name.Contains("toleft"))
            {
                if (path[i+1] == path[i] + Vector3Int.left)
                {
                    return path;
                } 
                if (path[i-1] == path[i] + Vector3Int.left)
                {
                    path.Reverse();
                    return path;
                }
            } else if (tilemap.GetTile(path[i]).name.Contains("toright"))
            {
                if (path[i+1] == path[i] + Vector3Int.right)
                {
                    return path;
                }
                if (path[i-1] == path[i] + Vector3Int.right)
                {
                    path.Reverse();
                    return path;
                }
            } else if (tilemap.GetTile(path[i]).name.Contains("toupleft"))
            {
                if (path[i+1] == path[i] + Vector3Int.left + Vector3Int.up)
                {
                    return path;
                } 
                if (path[i-1] == path[i] + Vector3Int.left + Vector3Int.up)
                {
                    path.Reverse();
                    return path;
                }
            } else if (tilemap.GetTile(path[i]).name.Contains("toupright"))
            {
                if (path[i+1] == path[i] + Vector3Int.right + Vector3Int.up)
                {
                    return path;
                } 
                if (path[i-1] == path[i] + Vector3Int.right + Vector3Int.up)
                {
                    path.Reverse();
                    return path;
                }
            }
        }
        return new List<Vector3Int>();
    }

    static private Vector3Int FindUniqueTileWithName(string name, Dictionary<Vector3Int, List<Vector3Int>> connections, 
        Tilemap tilemap)
    {
        Vector3Int tileCoordinates = new Vector3Int(10000, 10000, 10000);
        foreach (var entry in connections)
        {
            if (tilemap.GetTile(entry.Key).name == name)
            {
                tileCoordinates = entry.Key;
                break;
            }
        }

        return tileCoordinates;
    }

    static private List<List<Vector3Int>> FindPathsToHeart(List<List<Vector3Int>> paths, Vector3Int startingPoint, 
        Vector3Int heart, Dictionary<Vector3Int, List<Vector3Int>> connections, Tilemap tilemap)
    {
        if (paths.Count == 0)
        {
            paths.Add(new List<Vector3Int>());
            paths[0].Add(startingPoint);
        }

        Vector3Int back = new Vector3Int(10000,10000,10000);
        if (paths[paths.Count - 1].Count > 1)
        {
            back = paths[paths.Count - 1][paths[paths.Count - 1].Count - 2];
        }

        int currentIndex = paths.Count - 1;
        for (int i = 0; i < connections[startingPoint].Count; i++)
        {
            if (connections[startingPoint][i] == heart)
            {
                paths[paths.Count - 1].Add(connections[startingPoint][i]);
            } else if (connections[startingPoint][i] != back && !paths[paths.Count - 1].Contains(connections[startingPoint][i]) 
                                                             && tilemap.GetTile(connections[startingPoint][i]).name.Contains("flow"))
            {
                paths.Add(new List<Vector3Int>(paths[currentIndex]));

                paths[paths.Count - 1].Add(connections[startingPoint][i]);
                FindPathsToHeart(paths, connections[startingPoint][i], heart, connections, tilemap);
            }
        } 

        return paths;
    }

    static private Dictionary<Vector3Int, List<Vector3Int>> MergeConnections(Dictionary<Vector3Int, List<Vector3Int>> connectionsA,
        Dictionary<Vector3Int, List<Vector3Int>> connectionsB)
    {
        Dictionary<Vector3Int, List<Vector3Int>> allConnections = new Dictionary<Vector3Int, List<Vector3Int>>();
        foreach (var entry in connectionsA)
        {
            allConnections.Add(entry.Key, new List<Vector3Int>());
            foreach (var vector in entry.Value)
            {
                allConnections[entry.Key].Add(vector);
            }
        }
       
        foreach (var entry in connectionsB)
        {
            if (!allConnections.ContainsKey(entry.Key))
            {
                allConnections.Add(entry.Key, new List<Vector3Int>());
                foreach (var vector in entry.Value)
                {
                    allConnections[entry.Key].Add(vector);
                }
            }
            else
            {
                foreach (var vertex in entry.Value)
                {
                    if (!allConnections[entry.Key].Contains(vertex))
                    {
                        allConnections[entry.Key].Add(vertex);
                    }
                }
            }
        }

        return allConnections;
    }

    static private Dictionary<Vector3Int, List<Vector3Int>> BuildConnectionsMap(List<List<Vector3Int>> listOfLines)
    {
        Dictionary<Vector3Int, List<Vector3Int>> connections = new Dictionary<Vector3Int, List<Vector3Int>>();

        for (int i = 0; i < listOfLines.Count; i++)
        {
            for (int j = 0; j < listOfLines[i].Count; j++)
            {
                if (listOfLines[i].Count > j + 1)
                {
                    //Put the connection from 1st to 2nd if not already present
                    if (!connections.ContainsKey(listOfLines[i][j]))
                    {
                        List<Vector3Int> newList = new List<Vector3Int>();
                        newList.Add(listOfLines[i][j+1]);
                        connections.Add(listOfLines[i][j], newList);
                    }
                    else if (!connections[listOfLines[i][j]].Contains(listOfLines[i][j+1]))
                    {
                        connections[listOfLines[i][j]].Add(listOfLines[i][j+1]);
                    }
                    
                    //Put the connection from 2nd to 1st if not present
                    if (!connections.ContainsKey(listOfLines[i][j+1]))
                    {
                        List<Vector3Int> newList = new List<Vector3Int>();
                        newList.Add(listOfLines[i][j]);
                        connections.Add(listOfLines[i][j+1], newList);
                    }
                    else if (!connections[listOfLines[i][j+1]].Contains(listOfLines[i][j]))
                    {
                        connections[listOfLines[i][j+1]].Add(listOfLines[i][j]);
                    }
                }
            }
        }

        return connections;
    }

    static private void PrintDictionary(Dictionary<Vector3Int, List<Vector3Int>> dictionary)
    {
        Debug.Log("Printing dictionary with " + dictionary.Count + " entries");
        foreach (var entry in dictionary)
        {
            string connections ="Connections to " + entry.Key + " = ";
            for (int i = 0; i < entry.Value.Count; i++)
            {
                connections += entry.Value[i] + "; ";
            }
            Debug.Log(connections);
        }
    }

    static private void PrintPaths(List<List<Vector3Int>> paths)
    {
        Debug.Log("Printing " + paths.Count + " Paths");
        for (int i = 0; i < paths.Count; i++)
        {
            string pathString = "path number " + (i + 1) + ":";
            for (int j = 0; j < paths[i].Count; j++)
            {
                pathString += paths[i][j] + " -> ";
            }
            Debug.Log(pathString);
        }
    }

    static private void PrintDictionaryKeys(Dictionary<Vector3Int, List<Vector3Int>> dictionary)
    {
        string output = "Printing dictionary with " + dictionary.Count + " entries: ";
        foreach (var key in dictionary)
        {
            output += key.Key + "; ";
        }
        Debug.Log(output);
    }
}