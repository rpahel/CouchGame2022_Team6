using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GridManager : MonoBehaviour {

    public GameObject gridPrefab;
    [SerializeField] private Vector2 viewGridSize;
    [SerializeField] private Vector2 gridSize;

    private static List<Tile> _tiles;
    private bool _isDragging;
    private GameObject _dragArea;

    private Vector3 _startAreaVec;

    private void Start() {
        _tiles = new List<Tile>();
        
        for (int i = 0; i < gridSize.y; i++) {
            for (int j = 0; j < gridSize.x; j++) {
                
                GameObject tile = Instantiate(gridPrefab,new Vector2(85 + gridPrefab.GetComponent<RectTransform>().rect.width * j,1000 - gridPrefab.GetComponent<RectTransform>().rect.height * i),Quaternion.identity,transform);
                Vector3 tilePosition = tile.transform.position;
                _tiles.Add(new Tile(tile,tilePosition.x,tilePosition.y));
                
                if(i > viewGridSize.y || j > viewGridSize.x)
                    tile.GetComponent<Image>().enabled = false;
            }
        }
    }

    public void OnMoveGrid(InputAction.CallbackContext e) {
        if (e.started) {
            for (int i = 0; i < transform.childCount; i++)
            {
                //transform.GetChild(i).
            }
            //transform.Translate(e.ReadValue<Vector2>() * 30);
        }
    }

    public class Tile {
        public GameObject tile { get; private set; }
        public float x;
        public float y;
        public GameObject content { get; set; }

        public Tile(GameObject _tile, float _x,float _y) {
            tile = _tile;
            x = _x;
            y = _y;
        }

        public static Tile FindTileAtCoords(Vector2 coords) {
            foreach (Tile tile in _tiles) {
                if (coords.x == tile.x && coords.y == tile.y)
                    return tile;
            }

            return null;
        }
    }
}
