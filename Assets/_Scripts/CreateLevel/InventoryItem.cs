using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour,IPointerClickHandler {

    public enum ItemType {
        BLOCKS,
        EMPLACEMENT,
        PLAYER
    }
    private GameObject _content;

    [SerializeField] private Transform _blocksMap;
    [SerializeField] private ItemType type;

    public GameObject _drag;
    private RectTransform _gridManager;

    [SerializeField] private InputActionAsset _inputs;
    
    private List<InventoryItem> _items;

    private void Awake() {
        _content = transform.GetChild(0).gameObject;
        _gridManager = FindObjectOfType<GridManager>().GetComponent<RectTransform>();
        _items = FindObjectsOfType<InventoryItem>().ToList();

        _inputs.FindActionMap("Menu").FindAction("Click").performed += OnClick;
        _inputs.FindActionMap("Game").FindAction("Pause").started += OnDeleteDrag;
    }


    private void Update() {
        if (_drag != null) {
            _drag.transform.position = Input.mousePosition;

            if (Input.GetMouseButton(0)) {
                if (type != ItemType.EMPLACEMENT) {
                    for (int i = 0; i < _gridManager.transform.childCount; i++) {
                        RectTransform gridRect = _gridManager.transform.GetChild(i).GetComponent<RectTransform>();

                        if (RectTransformUtility.RectangleContainsScreenPoint(gridRect, Input.mousePosition)) {
                            GridManager.Tile targetTile = GridManager.Tile.FindTileAtCoords(gridRect.transform.position);
                            
                            if(targetTile.content != null || !targetTile.tile.GetComponent<Image>().IsActive())
                                break;
                                
                            GameObject content = Instantiate(_content, gridRect.transform.position, Quaternion.identity,gridRect.transform);
                            targetTile.content = content;
                            break;
                        }
                    }
                }
                else {
                    for (int i = 0; i < _gridManager.transform.childCount; i++) {
                        RectTransform gridRect = _gridManager.transform.GetChild(i).GetComponent<RectTransform>();

                        if (RectTransformUtility.RectangleContainsScreenPoint(gridRect, Input.mousePosition)) {
                            GridManager.Tile targetTile = GridManager.Tile.FindTileAtCoords(gridRect.transform.position);

                            if (!targetTile.tile.GetComponent<Image>().IsActive()) 
                                targetTile.tile.GetComponent<Image>().enabled = true;
                        
                        }
                    }
                }
                
            }
        }
        else {
            List<InventoryItem> _drags = _items.Where(item => item._drag != null).ToList();
            
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && _drags.Count == 0) {
                for (int i = 0; i < _gridManager.transform.childCount; i++) {
                    RectTransform gridRect = _gridManager.transform.GetChild(i).GetComponent<RectTransform>();
                    if (RectTransformUtility.RectangleContainsScreenPoint(gridRect, Input.mousePosition)) {
                        GridManager.Tile targetTile = GridManager.Tile.FindTileAtCoords(gridRect.transform.position);
                        if (targetTile.content != null) {
                            Destroy(gridRect.transform.GetChild(0).gameObject); 
                            break;
                        }
                    }
                }
            }
        }

    }

    public void OnPointerClick(PointerEventData eventData) {
        if (_drag == null) {
            _drag = Instantiate(_content, _content.transform.position, Quaternion.identity, _blocksMap);
        }
    }

    public void OnClick(InputAction.CallbackContext e) {
        if (e.performed && _drag != null) {
            if (type != ItemType.EMPLACEMENT) {
            
                for (int i = 0; i < _gridManager.transform.childCount; i++) {
                    RectTransform gridRect = _gridManager.transform.GetChild(i).GetComponent<RectTransform>();

                    if (RectTransformUtility.RectangleContainsScreenPoint(gridRect, Input.mousePosition)) {
                        GridManager.Tile targetTile = GridManager.Tile.FindTileAtCoords(gridRect.transform.position);
                        
                        Debug.Log("log");
                        if (targetTile.content != null || !targetTile.tile.GetComponent<Image>().IsActive())
                            break;
                        
                        Debug.Log("active " + targetTile.tile.GetComponent<Image>().IsActive());

                        GameObject content = Instantiate(_content, gridRect.transform.position, Quaternion.identity,gridRect.transform);
                        targetTile.content = content;
                        break;
                    }
                }
            }
            else {
                for (int i = 0; i < _gridManager.transform.childCount; i++) {
                    RectTransform gridRect = _gridManager.transform.GetChild(i).GetComponent<RectTransform>();

                    if (RectTransformUtility.RectangleContainsScreenPoint(gridRect, Input.mousePosition)) {
                        GridManager.Tile targetTile = GridManager.Tile.FindTileAtCoords(gridRect.transform.position);

                        if (!targetTile.tile.GetComponent<Image>().IsActive()) 
                            targetTile.tile.GetComponent<Image>().enabled = true;
                        
                    }
                }
            }
        }
    }

    public void OnDeleteDrag(InputAction.CallbackContext e) { // Pause Button
        if (e.started) {
            if (_drag != null) {
                Destroy(_drag);
                _drag = null;
            }
        }
    }
}
