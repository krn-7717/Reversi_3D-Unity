using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthelloCharacter : MonoBehaviour
{
    [SerializeField] Material _material = null;

    Material _myMaterialA = null;
    Material _myMaterialB = null;

    [SerializeField] MeshRenderer _cylinderA = null;
    [SerializeField] MeshRenderer _cylinderB = null;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(bool isFace) {
        if (_myMaterialA == null) {
            _myMaterialA = GameObject.Instantiate<Material>(_material);
            _myMaterialB = GameObject.Instantiate<Material>(_material);
            _cylinderA.material = _myMaterialA;
            _cylinderB.material = _myMaterialB;
        }
        _myMaterialA.color = isFace ? Color.white : Color.black;
        _myMaterialB.color = isFace ? Color.black : Color.white;
    }

    public void SetState(OthelloSystem.eCharacterState state) {
        bool isActive = (state != OthelloSystem.eCharacterState.None);
        {   
            _cylinderA.gameObject.SetActive(isActive);
            _cylinderB.gameObject.SetActive(isActive);
        }
        SetColor(state == OthelloSystem.eCharacterState.Face);
    }
}
