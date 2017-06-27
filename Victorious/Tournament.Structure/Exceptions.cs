using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Structure
{
	public class DuplicateObjectException : Exception, ISerializable
	{
		public DuplicateObjectException() { }
		public DuplicateObjectException(string message) { }
		public DuplicateObjectException(string message, Exception inner) { }

		protected DuplicateObjectException(SerializationInfo info, StreamingContext context) { }
	}

	public class InactiveMatchException : Exception, ISerializable
	{
		public InactiveMatchException() { }
		public InactiveMatchException(string message) { }
		public InactiveMatchException(string message, Exception inner) { }

		protected InactiveMatchException(SerializationInfo info, StreamingContext context) { }
	}

	public class SlotFullException : Exception, ISerializable
	{
		public SlotFullException() { }
		public SlotFullException(string message) { }
		public SlotFullException(string message, Exception inner) { }

		protected SlotFullException(SerializationInfo info, StreamingContext context) { }
	}

	public class AlreadyAssignedException : Exception, ISerializable
	{
		public AlreadyAssignedException() { }
		public AlreadyAssignedException(string message) { }
		public AlreadyAssignedException(string message, Exception inner) { }

		protected AlreadyAssignedException(SerializationInfo info, StreamingContext context) { }
	}

	public class InvalidIndexException : Exception, ISerializable
	{
		public InvalidIndexException() { }
		public InvalidIndexException(string message) { }
		public InvalidIndexException(string message, Exception inner) { }

		protected InvalidIndexException(SerializationInfo info, StreamingContext context) { }
	}
	public class InvalidSlotException : Exception, ISerializable
	{
		public InvalidSlotException() { }
		public InvalidSlotException(string message) { }
		public InvalidSlotException(string message, Exception inner) { }

		protected InvalidSlotException(SerializationInfo info, StreamingContext context) { }
	}

	public class GameNotFoundException : Exception, ISerializable
	{
		public GameNotFoundException() { }
		public GameNotFoundException(string message) { }
		public GameNotFoundException(string message, Exception inner) { }

		protected GameNotFoundException(SerializationInfo info, StreamingContext context) { }
	}
	public class MatchNotFoundException : Exception, ISerializable
	{
		public MatchNotFoundException() { }
		public MatchNotFoundException(string message) { }
		public MatchNotFoundException(string message, Exception inner) { }

		protected MatchNotFoundException(SerializationInfo info, StreamingContext context) { }
	}
	public class PlayerNotFoundException : Exception, ISerializable
	{
		public PlayerNotFoundException() { }
		public PlayerNotFoundException(string message) { }
		public PlayerNotFoundException(string message, Exception inner) { }

		protected PlayerNotFoundException(SerializationInfo info, StreamingContext context) { }
	}
	public class BracketNotFoundException : Exception, ISerializable
	{
		public BracketNotFoundException() { }
		public BracketNotFoundException(string message) { }
		public BracketNotFoundException(string message, Exception inner) { }

		protected BracketNotFoundException(SerializationInfo info, StreamingContext context) { }
	}

	public class ScoreException : Exception, ISerializable
	{
		public ScoreException() { }
		public ScoreException(string message) { }
		public ScoreException(string message, Exception inner) { }

		protected ScoreException(SerializationInfo info, StreamingContext context) { }
	}
	public class BracketException : Exception, ISerializable
	{
		public BracketException() { }
		public BracketException(string message) { }
		public BracketException(string message, Exception inner) { }

		protected BracketException(SerializationInfo info, StreamingContext context) { }
	}
	public class BracketValidationException : Exception, ISerializable
	{
		public BracketValidationException() { }
		public BracketValidationException(string message) { }
		public BracketValidationException(string message, Exception inner) { }

		protected BracketValidationException(SerializationInfo info, StreamingContext context) { }
	}
}
