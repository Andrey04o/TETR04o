using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oBagSystem : UdonSharpBehaviour
    {
        public T04oGameProcess gameProcess;
        public byte indexCount; //synced
        byte _indexCount;
        public byte bagCount; //synced
        byte _bagCount;
        int[] currentBag;
        int randomizer = 0;
        int _randomizer = 0;
        int pieceCount = 7;
        
        public byte ChooseRandomPiece() {
            if (_randomizer != randomizer) {
                InitArray(gameProcess.pieces, gameProcess.randomizer, false);
            }
            if (currentBag == null) {
                InitArray(gameProcess.pieces, gameProcess.randomizer, false);
            }
            if (_bagCount != bagCount) {
                GenerateBag();
            }
            if (indexCount >= currentBag.Length) {
                GenerateBag();
            }
           // _indexCount++;
            return (byte)currentBag[indexCount++];
        }

        public void InitArray(T04oPiece[] pieces, int randomizer, bool reset) { // for example, we will input null,i,s,z,o,t,l,j array and a 92340234012 randomizer number
            pieceCount = pieces.Length - 1;
            currentBag = new int[pieceCount];
            this.randomizer = randomizer;
            _randomizer = randomizer;
            if (reset)
                bagCount = 0;
            GenerateBag();
        }

        public void GenerateBag() {
            indexCount = 0;
            
            for (int i = 0; i < pieceCount; i++) {
                currentBag[i] = i + 1;
            }
            
            int seed = randomizer + bagCount;
            
            for (int i = pieceCount - 1; i > 0; i--) {
                seed = (seed * 1103515245 + 12345) & 0x7fffffff;
                int j = seed % (i + 1);
                
                int temp = currentBag[i];
                currentBag[i] = currentBag[j];
                currentBag[j] = temp;
            }
            
            bagCount++;
            _bagCount = bagCount;
        }

    }
}
