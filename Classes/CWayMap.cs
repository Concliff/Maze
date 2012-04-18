using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Maze.Classes
{
    public class CWayMap
    {
       public struct CMapChunk
       {
           public int Passability;
       };

       // Screen coordinates
       public struct MapPoint
       {
           public int x;
           public int z;
       };

       public struct WayChunk
       {
           public int ID; // The number of a cell
           public int MasterID; // Cell's parent
           public int G; // Cost of movement from the start point to the current one
           public int H; // The cost of movement from the current point to the final one (by Manhattan method)           
           public int F; //sum of G and H
       };

       // Verification is required
       static int NO_INIT = -1;
   
       // Grid
       public CMapChunk[] m_Grid;

       // A counter of zones
       private int m_iCountZone;

       public CWayMap()
       {
           m_Grid = new CMapChunk [GlobalConstants.GRIDMAP_BLOCK_WIDTH * GlobalConstants.GRIDMAP_BLOCK_HEIGHT];
           
           for(int x = 0; x < GlobalConstants.GRIDMAP_BLOCK_WIDTH * GlobalConstants.GRIDMAP_BLOCK_HEIGHT; ++x)
           {
               m_Grid[x].Passability = NO_INIT;
           }
           
           m_iCountZone = 0;
           
           SplitZones(NO_INIT);
       }

       // Division the cells with a certain type of passableness into zones
       private void SplitZones(int iPassabilityType)
       {
           for(int x = 0; x < GlobalConstants.GRIDMAP_BLOCK_WIDTH * GlobalConstants.GRIDMAP_BLOCK_HEIGHT; ++x)
           {
               if (m_Grid[x].Passability == iPassabilityType)
               {
                   // Increase number of zones
                   if (++m_iCountZone > int.MaxValue - 1)
                       MessageBox.Show("ошибка, счетчик переполнится и будет баг");
                   
                   // Create a new zone
                   CreateZone(x, m_iCountZone);
               }
           }
       }
       
       private void CreateZone(int iChunkID, int NewZone)
       {
           // The list of cells' numbers, for which it necessary to find all neighbors
           List<int> aListForCheck = new List<int> ();

           aListForCheck.Add(iChunkID);

           while (aListForCheck.Count != 0)
           {
               int iChunkX = aListForCheck[0] % GlobalConstants.GRIDMAP_BLOCK_WIDTH;
               int iChunkZ = aListForCheck[0] / GlobalConstants.GRIDMAP_BLOCK_WIDTH;
               
               for (int i = -1; i <= 1; i++)
               {
                   for (int j = -1; j <= 1; j++)
                   {
                       int iNeighborX = iChunkX + i;
                       int iNeighborZ = iChunkZ + j;
                       
                       if (CheckPassability(iNeighborX, iNeighborZ))
                       {
                           int iNeighborID = GetIDByPos(iNeighborX, iNeighborZ);
                           
                           if (m_Grid[iNeighborID].Passability != NewZone)
                           {
                               if (CheckDiagonals(iNeighborX, iNeighborZ, iChunkX, iChunkZ))
                               {
                                   m_Grid[iNeighborID].Passability = NewZone;
                                   aListForCheck.Add(iNeighborID);
                               }
                           }
                       }
                   }
               }

               // Delete checked cell from the list
               aListForCheck.Remove(aListForCheck[0]);
           }
       }

       public bool CheckDiagonals(int iNeighborX, int iNeighborZ, int iMasterX, int iMasterZ)
       {
           int iDiffX = iNeighborX - iMasterX;
           int iDiffZ = iNeighborZ - iMasterZ;

           // If the neighbor is diagonal
           if (iDiffX == 0 && iDiffZ == 0)
           {
               // Find the first neighbor to neighbor contact with the "master"
               int iContact1X = iMasterX + iDiffX;
               int iContact1Y = iMasterZ;

               // If found neighbor is not passable, return false
               if (m_Grid[GetIDByPos(iContact1X, iContact1Y)].Passability == 0)
                   return false;

               // Find the second neighbor to neighbor contact with the "master"
               int iContact2X = iMasterX;
               int iContact2Y = iMasterZ + iDiffZ;

               // If found neighbor is not passable, return false
               if (m_Grid[GetIDByPos(iContact2X, iContact2Y)].Passability == 0)
                   return false;
           }
           // else the neighbor of the "master" is passable
           return true;
       }

       public bool CheckRange(int iChunkX, int iChunkZ)
       {
           if (iChunkX < 0 || iChunkZ < 0 || iChunkX > GlobalConstants.GRIDMAP_BLOCK_WIDTH - 1 || iChunkZ > GlobalConstants.GRIDMAP_BLOCK_HEIGHT - 1)
               return false;
           return true;
       }

       public void ChangePassability(int x, int z)
       {
           // x and z are screen coordinates of mouse click
           int iChunkX = x/GlobalConstants.GRIDMAP_BLOCK_WIDTH;
           int iChunkZ = z/GlobalConstants.GRIDMAP_BLOCK_HEIGHT;
           
           int iChunkID = GetIDByPos(iChunkX, iChunkZ);
 
           int iChangePassability = m_Grid[iChunkID].Passability;
           
           if (iChangePassability == 0)
           {
              iChangePassability = NO_INIT;
              m_Grid[iChunkID].Passability = NO_INIT;
           }
           else
              m_Grid[iChunkID].Passability = 0;
 
           List <int> aZoneForSplit = new List<int> ();

           // The number of found impassable neighbors  
           int iCountBlock = 0;

           for (int i = -1; i <= 1; i++)
           {
              for (int j = -1; j <= 1; j++)
              {
                 if (j == 0 && i == 0)
                    continue;
 
                 int iNeighborX = iChunkX+i;
                 int iNeighborZ = iChunkZ+j;
                 
                 int iNeighborID = GetIDByPos(iNeighborX, iNeighborZ);
                 
                 if (CheckPassability(iNeighborX, iNeighborZ))
                 {
                    if (iChangePassability == NO_INIT)
                    {
                       // If zone is unique
                       bool bNewSplitZone = true;
                       for (int ii = 0; ii < aZoneForSplit.Count; ii++)
                       {
                          // If zone is in the list already
                          if (aZoneForSplit[i] == m_Grid[iNeighborID].Passability)
                             bNewSplitZone = false;
                       }

                       if (bNewSplitZone)
                          aZoneForSplit.Add(m_Grid[iNeighborID].Passability);
     
                       m_Grid[iChunkID].Passability = m_Grid[iNeighborID].Passability;
                    }
                 }
                 else
                    iCountBlock++; 
 
                 if (iCountBlock > 1)
                 {
                    if (iChangePassability == NO_INIT)
                    {
                       for (int xx = 0; xx < (GlobalConstants.GRIDMAP_BLOCK_WIDTH * GlobalConstants.GRIDMAP_BLOCK_HEIGHT); xx++)
                       {
                          for (int ii = 0; ii < aZoneForSplit.Count; ii++ )
                          {
                             if (aZoneForSplit[i] == m_Grid[xx].Passability)
                             {
                                m_Grid[xx].Passability = NO_INIT;
                                break;
                             }
                          }
                       }
                    }
                    
                    SplitZones(iChangePassability);
                    
                  }
              }
           }
       }
       
       public int GetIDByPos(int iChunkX, int iChunkZ)
       {
           return iChunkZ * GlobalConstants.GRIDMAP_BLOCK_WIDTH + iChunkX;
       }

       public bool CheckPassability(int iChunkX, int iChunkZ)
       {
           if (CheckRange(iChunkX, iChunkZ))
           {
               if (m_Grid[GetIDByPos(iChunkX, iChunkZ)].Passability != 0)
                   return true;
           }
           return false;
       }

       public bool IsCanWay(int iStartID, int iEndID)
       {
           if (iStartID == iEndID)
               return false;
           
           if (m_Grid[iStartID].Passability != m_Grid[iEndID].Passability)
               return false;
           
           if (m_Grid[iEndID].Passability == 0)
               return false;
           
           return true;
       }

      public int GetGridWidth() { return GlobalConstants.GRIDMAP_BLOCK_WIDTH; } 

      public int GetChunkSize() { return GlobalConstants.GRIDMAP_BLOCK_HEIGHT; }
    }
 
       
}
