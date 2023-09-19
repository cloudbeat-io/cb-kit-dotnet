using System;
using System.Collections.Generic;

namespace CloudBeat.Kit.Common.Models
{
	public interface IResultWithAttachment
	{
		IList<Attachment> Attachments { get; set; }
	}
}

