using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.Models
{
    class UndoRedoStack : MainWindow
    {
        private Stack<Vertex> _undoForVertex;
        private Stack<Vertex> _redoForVertex;
        private Stack<Edges> _undoForEdges;
        private Stack<Edges> _redoForEdges;
        public Stack<string> stackOfUndoOperation;
        public Stack<string> stackOfRedoOperation;
        public UndoRedoStack()
        {
            Reset();
        }
        public int UndoCountForVertex
        {
            get
            {
                return _undoForVertex.Count;
            }
        }
        public int RedoCountForVertex
        {
            get
            {
                return _redoForVertex.Count;
            }
        }
        public int UndoCountForEdges
        {
            get
            {
                return _undoForEdges.Count;
            }
        }
        public int RedoCountForEdges
        {
            get
            {
                return _redoForEdges.Count;
            }
        }
        public void Reset()
        {
            _undoForVertex = new Stack<Vertex>();
            _redoForVertex = new Stack<Vertex>();
            _undoForEdges = new Stack<Edges>();
            _redoForEdges = new Stack<Edges>();
            stackOfUndoOperation = new Stack<string>();
            stackOfRedoOperation = new Stack<string>();
        }
        public void Execute(Vertex vertex)
        {
            _undoForVertex.Push(vertex);

            _redoForVertex.Clear();
        }
        public void Execute(Edges edge)
        {
            _undoForEdges.Push(edge);

            _redoForEdges.Clear();
        }
        public Vertex Undo(Vertex vertex)
        {
            if (_undoForVertex.Count > 0)
            {
                Vertex prevVertex = _undoForVertex.Pop();

                _redoForVertex.Push(prevVertex);

                return prevVertex;
            }
            else
            {
                return vertex;
            }
        }
        public Edges Undo()
        {
            if (_undoForEdges.Count > 0)
            {
                Edges prevEdge = _undoForEdges.Pop();

                _redoForEdges.Push(prevEdge);

                return prevEdge;
            }
            else
            {
                return null;
            }
        }
        public Vertex Redo()
        {

            if (_redoForVertex.Count > 0)
            {
                Vertex prevVertex = new Vertex();

                prevVertex = _redoForVertex.Pop();

                _undoForVertex.Push(prevVertex);
                return prevVertex;
            }
            else
            {
                return null;
            }
        }
        public Edges RedoForEdges()
        {

            if (_redoForEdges.Count > 0)
            {
                Edges prevEdge = new Edges();

                prevEdge = _redoForEdges.Pop();

                _undoForEdges.Push(prevEdge);
                return prevEdge;
            }
            else
            {
                return null;
            }
        }
    }
}
