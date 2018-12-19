using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SnapingEditorWindow : EditorWindow
{
    private bool      _snap   = false;
    private int       _size   = 1;
    private Vector3   _offset = Vector3.zero;
    private Transform _selected_transform;
    private Vector3   _selected_transform_position;

    private void OnEnable()
    {
        Undo.undoRedoPerformed += OnUndoRedoPerformed;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedoPerformed;
    }

    private void OnGUI()
    {
        _snap = EditorGUILayout.Toggle("Snap", _snap);
        _size = EditorGUILayout.IntField("Size", _size);
        _offset = EditorGUILayout.Vector3Field("Offset", _offset);
    }

    [MenuItem("Tools/Snapping")]
    public static void Open()
    {
        GetWindow<SnapingEditorWindow>();
    }

    private void OnUndoRedoPerformed()
    {
        if (_selected_transform)
        {
            _selected_transform_position = _selected_transform.position;
        }
    }

    private void Update()
    {
        if (_snap)
        {
            var selected_transform = Selection.activeTransform;
            if (selected_transform)
            {
                if (selected_transform != _selected_transform)
                {
                    _selected_transform = selected_transform;
                    _selected_transform_position = selected_transform.position;
                }
                else
                {
                    if (_selected_transform_position != _selected_transform.position)
                    { // transform moved, can snap
                        _selected_transform_position = selected_transform.position;
                        _selected_transform_position = new Vector3(
                            Mathf.Round((_selected_transform_position.x - _offset.x) / _size) * _size + _offset.x,
                            Mathf.Round((_selected_transform_position.y - _offset.y) / _size) * _size + _offset.y,
                            Mathf.Round((_selected_transform_position.z - _offset.z) / _size) * _size + _offset.z
                        );
                        _selected_transform.position = _selected_transform_position;
                        Undo.RecordObject(_selected_transform, "Snap");
                    }
                }
            }
            else
            {
                _selected_transform_position = Vector3.zero;
                _selected_transform          = null;
            }
        }
    }
}
