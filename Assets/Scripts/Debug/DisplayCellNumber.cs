using UnityEngine;

public class DisplayCellNumber : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    
    private Camera _camera;
    private Plane _plane;
    private Vector3Int _cell;
    private Ray _ray;
    private Vector3 _intersectionPoint;

#if UNITY_EDITOR
    void Start()
    {
        _camera = Camera.main;
        
        Debug.Log("Vector3.up: " + Vector3.up);
        
        _plane = new Plane(-Vector3.up, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("mouse pos: " + Input.mousePosition);
            _ray = _camera.ScreenPointToRay(Input.mousePosition);

            var ray = _ray;
            if (_plane.Raycast(ray, out var enter))
            {
                _intersectionPoint = ray.GetPoint(enter);
                _cell = _grid.WorldToCell(_intersectionPoint);
                
                Debug.Log("Raycast cell: " + _cell);
            }
        }
    }
    
#endif
}
