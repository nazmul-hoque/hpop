/*
*Name:			OpenPOP.MIMEParser.DecodeQP
*Function:		Decoding Quoted-Printable text
*Author:		
*Created:		2003/8
*Modified:		2004/3/29 12:33 GMT+8
*Description	:
*Changes:		
*/

using System;
using System.Text;
using System.Globalization;

namespace OpenPOP.MIMEParser
{
	/// <summary>
	/// Decoding Quoted-Printable text
	/// 
	/// </summary>
	public class DecodeQP
	{
		public DecodeQP()
		{
		}

		/// <summary>
		/// Decoding Quoted-Printable string
		/// </summary>
		/// <param name="HexString">Quoted-Printable encoded string</param>
		/// <param name="encode">encoding method</param>
		/// <returns>decoded string</returns>
		private static string ConvertHexToString(string HexString,Encoding encode)
		{			
			try
			{
				if(HexString==null||HexString.Equals("")) return "";

				if(HexString.StartsWith("=")) HexString=HexString.Substring(1);
			
				string[] aHex= HexString.Split(new char[1]{'='});
				byte[] abyte = new Byte[aHex.Length];
			
				for(int i=0;i<abyte.Length;i++)
				{
					//	Console.WriteLine(aHex[i]);
					abyte[i] =(byte) int.Parse(aHex[i],NumberStyles.HexNumber);
				}
				return encode.GetString(abyte);
			}
			catch
			{
				return HexString;
			}
		}

		/// <summary>
		/// Decoding Quoted-Printable string at a position
		/// </summary>
		/// <param name="HexString">Quoted-Printable encoded string</param>
		/// <param name="encode">encoding method, "Default" is suggested</param>
		/// <param name="nStart">position to start, normally 0</param>
		/// <returns>decoded string</returns>
		public static string ConvertHexContent(string HexString,Encoding encode,long nStart)
		{			
			if(nStart>=HexString.Length) return HexString;

			//to hold string to be decoded
			StringBuilder sbHex = new  StringBuilder();
			sbHex.Append("");
			//to hold decoded string
			StringBuilder sbEncoded = new StringBuilder();
			sbEncoded.Append("");
			//wether we reach Quoted-Printable string
			bool isBegin = false;
			string temp;
			int i = (int)nStart;

			while(i<HexString.Length )
			{
				//init next loop
				sbHex.Remove(0,sbHex.Length);
				isBegin = false;
				int count=0;

				while(i<HexString.Length )
				{
					temp = HexString.Substring(i,1);//before reaching Quoted-Printable string, one char at a time
					if(temp.StartsWith("=")) 
					{
						temp = HexString.Substring(i,3);//get 3 chars
						if(temp.EndsWith("\r\n"))//return char
						{
							if(isBegin&& (count % 2==0))
								break;								
							//	sbEncoded.Append("");
							i=i+3;
						}
						else if(!temp.EndsWith("3D"))
						{
							sbHex.Append(temp);
							isBegin = true;//we reach Quoted-Printable string, put it into buffer
							i=i+3;
							count++;
						}
						else //if it ends with 3D, it is "="
						{
							if(isBegin&& (count % 2==0)) //wait until even items to handle all character sets
								break;	
							
							sbEncoded.Append("=");
							i=i+3;
						}

					}
					else
					{
						if(isBegin)//we have got the how Quoted-Printable string, break it
							break;	
						sbEncoded.Append(temp);//not Quoted-Printable string, put it into buffer
						i++;
					}

				}
				//decode Quoted-Printable string
				sbEncoded.Append(ConvertHexToString(sbHex.ToString(),encode)); 
			}
			
			return sbEncoded.ToString();		
		}


		/// <summary>
		/// Decoding Quoted-Printable string using default encoding and begin at 0
		/// </summary>
		/// <param name="HexString">Quoted-Printable encoded string</param>
		/// <returns>decoded string</returns>
		public static string ConvertHexContent(string HexString)
		{
			if(HexString==null || HexString.Equals("")) return HexString;

			return ConvertHexContent(HexString,Encoding.Default,0);
			
		}
	}
}
