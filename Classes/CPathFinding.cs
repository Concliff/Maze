using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Maze.Classes
{
    class CPathFinding
    {
       // cost of the horizontal and vertical movement
       private static int WeightLine = 10;
       // cost of diagonal movement
       private static int WeightDiag = 14;
       
       private enum PossibleResults
       { 
          SEARTH_COMPLETED = -1, 
          NOT_FIND_END = -2,
          FIND_END = -3 
       };
       
       public CWayMap WayMap;
       // The list which should be checked
       private List <CWayMap.WayChunk> m_aOpenList = new List<CWayMap.WayChunk> ();
       // Checked list
       private List <CWayMap.WayChunk> m_aCloseList = new List<CWayMap.WayChunk> ();
 
       // The final point
       private int iEndX;
       private int iEndZ;

       // Search in the open list of point which has min F for its further analysis
       // (A serial number of the open list is returns!)
       private int GetMinWayChunk()
       {
           // If the open list is empty the way isn't found
           if (m_aOpenList.Count == 0) 
              return (int)PossibleResults.NOT_FIND_END;
 
           // Search in the open list of block with min F
           int iMinWay = m_aOpenList[0].F;
           int ID = 0;
           for (int i=0; i < m_aOpenList.Count; i++)
           {
              if (m_aOpenList[i].F < iMinWay) 
              {
                 iMinWay = m_aOpenList[i].F;
                 ID = i;
              }
           } 

           // If H of this block isn't 0 this is not the end of the way
           if (m_aOpenList[ID].H != 0)
              return ID;
           else
           {
              // else add the final point
              m_aCloseList.Add(m_aOpenList[ID]);
              return (int)PossibleResults.FIND_END;
           }
       }

       // Search the neighbors of a cell
       private bool DoSearchStep(int iChunkNum)
       {
           // Parent ID
           int iMasterID = m_aOpenList[iChunkNum].ID;

           // Width and height of the grid cells are calculated by number
           int iMasterX = iMasterID % WayMap.GetGridWidth();
           int iMasterZ = iMasterID / WayMap.GetGridWidth();

           // Look through all the neighbors
           for (int i = -1; i <= 1; i++)
           {
               for (int j = -1; j <= 1; j++)
               {
                   // If this is a cell, the neighbors of which are looking for, skip it
                   if (j == 0 && i == 0)
                       continue;

                   int iNeighborX = iMasterX + i;
                   int iNeighborZ = iMasterZ + j;

                   // Check of the range and passability of a cell
                   if (WayMap.CheckPassability(iNeighborX, iNeighborZ))
                   {
                       // Is it possible to pass not cut corners
                       if (WayMap.CheckDiagonals(iNeighborX, iNeighborZ, iMasterX, iMasterZ))
                       {
                           // Find the neighbor
                           int iNeighborID = WayMap.GetIDByPos(iNeighborX, iNeighborZ);

                           // If the cell was checked skip it
                           if (FindInCloseList(iNeighborID) > -1)
                               continue;

                           // Estimate the  way to this point
                           CWayMap.WayChunk Chunk;
                           
                           Chunk.ID = iNeighborID;
                           Chunk.MasterID = iMasterID;

                           // The cost of movement (by Manhattan method)
                           Chunk.H = (Math.Abs(iNeighborX - iEndX) + Math.Abs(iNeighborZ - iEndZ)) * 10;

                           // Calculate the length of the path from start point to the current
                           int iDiffX = Math.Abs(iMasterX - iNeighborX);
                           int iDiffZ = Math.Abs(iMasterZ - iNeighborZ);
                           int iDiag = Math.Min(iDiffX, iDiffZ);
                           int iDirect = Math.Max(iDiffX, iDiffZ) - Math.Min(iDiffX, iDiffZ);
                           
                           Chunk.G = iDiag * WeightDiag + iDirect * WeightLine + m_aOpenList[iChunkNum].G;
                           
                           Chunk.F = Chunk.H + Chunk.G;

                           int iNumChunk = FindInOpenList(Chunk.ID);
                           if (iNumChunk == -1)
                               // if not add it
                               m_aOpenList.Add(Chunk);
                           else
                           {
                               // if the new calculation is shorter than earlier one
                               if (m_aOpenList[iNumChunk].G > Chunk.G)
                                   m_aOpenList[iNumChunk] = Chunk;
                           }
                       }
                   }
               }
           }
           
           // Add the cell to the close list
           m_aCloseList.Add(m_aOpenList[iChunkNum]);
           m_aOpenList.Remove(m_aOpenList[0 + iChunkNum]);
           return true;
       }

       private int FindInOpenList(int iChunkID)
       {
           for (int i=0; i < m_aOpenList.Count; i++)
           {
              if (iChunkID == m_aOpenList[i].ID) 
                 return i;
           }
           return -1;
       }

       private int FindInCloseList(int iChunkID)
       {
           for (int i=0; i < m_aCloseList.Count; i++)
           {
              if (iChunkID == m_aCloseList[i].ID) 
                 return i;
           }
           return -1;
       }

       private int GetMasterID(int iChunkID)
       {
           for (int i=0; i < m_aCloseList.Count; i++)
           {
              if (m_aCloseList[i].ID == iChunkID) 
                 return m_aCloseList[i].MasterID;
           } 
           return -1;
       }

       void CPathfinding(int iWidth, int iHeight, int iSize)
       {
           WayMap = null;
           // Create a map
           WayMap = new CWayMap();
       }

       public bool GetWay(CWayMap.MapPoint From, CWayMap.MapPoint To, List<CWayMap.MapPoint> Way)
       {
           Way.Clear();

           // Transfer the starting point to the grid coordinates
           int iStartX = From.x / WayMap.GetChunkSize();
           int iStartZ = From.z / WayMap.GetChunkSize();
           
           if (!WayMap.CheckRange(iStartX, iStartZ))
               return false;

           // Transfer the ending point to the grid coordinates
           iEndX = To.x / WayMap.GetChunkSize();
           iEndZ = To.z / WayMap.GetChunkSize();
           
           if (!WayMap.CheckRange(iEndX, iEndZ))
               return false;

           int iIDFromChunk = WayMap.GetIDByPos(iStartX, iStartZ);
           int iIDToChunk = WayMap.GetIDByPos(iEndX, iEndZ);
           
           if (!WayMap.IsCanWay(iIDFromChunk, iIDToChunk))
               return false;
           
           // Add to the open list the starting point
           CWayMap.WayChunk Chunk;
           Chunk.ID = iIDFromChunk;
           Chunk.MasterID = iIDFromChunk;
           Chunk.G = 0;
           Chunk.H = (Math.Abs(iStartX - iEndX) + Math.Abs(iStartZ - iEndZ)) * WeightLine;
           Chunk.F = Chunk.G + Chunk.H;
           m_aOpenList.Add(Chunk);

           // Find the number of element with min F
           int iNumInOpenList = GetMinWayChunk();
           
           for (; iNumInOpenList > (int)PossibleResults.SEARTH_COMPLETED; iNumInOpenList = GetMinWayChunk()) //???????????????????????????????????
           {
               DoSearchStep(iNumInOpenList);
           }

           bool bDone = false;
           // If the end point is found
           if (iNumInOpenList == (int)PossibleResults.FIND_END)
           {
               CWayMap.MapPoint PointOfWay;
               PointOfWay.x = iEndX * WayMap.GetChunkSize();
               PointOfWay.z = iEndZ * WayMap.GetChunkSize();
               int ID = -1;
               int iMasterID = WayMap.GetIDByPos(iEndX, iEndZ);

               // Until in the reverse order the starting point will be not found
               // movement: from the end to the start
               while (ID != iMasterID)
               {
                   ID = iMasterID;
                   iMasterID = GetMasterID(ID);
                   PointOfWay.x = ID % WayMap.GetGridWidth() * WayMap.GetChunkSize();
                   PointOfWay.z = ID / WayMap.GetGridWidth() * WayMap.GetChunkSize();
                   Way.Add(PointOfWay);
               }
               bDone = true;
           }
           
           m_aCloseList.Clear();
           m_aOpenList.Clear();
           return bDone;
       }
    }
}
