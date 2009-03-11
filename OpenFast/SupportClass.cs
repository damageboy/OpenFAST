/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>

*/
using System;

public interface IThreadRunnable
{
    void Run();
}

public interface XmlSaxErrorHandler
{
    void error(System.Xml.XmlException exception);
    void fatalError(System.Xml.XmlException exception);
    void warning(System.Xml.XmlException exception);
}


public class XmlSourceSupport
{
    public XmlSourceSupport()
    {
        Bytes = null;
        Characters = null;
        Uri = null;
    }

    public XmlSourceSupport(System.IO.Stream stream)
    {
        Bytes = stream;
        Characters = null;
        Uri = null;
    }

    public XmlSourceSupport(System.IO.StreamReader reader)
    {
        Bytes = null;
        Characters = reader;
        Uri = null;
    }

    public XmlSourceSupport(string source)
    {
        Bytes = null;
        Characters = null;
        Uri = source;
    }

    public System.IO.Stream Bytes { get; set; }

    public System.IO.StreamReader Characters { get; set; }

    public string Uri { get; set; }
}

public class SupportClass
{
    public class StackSupport
    {

        public static Object Pop(System.Collections.ArrayList stack)
        {
            System.Object obj = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            return obj;
        }
    }


    public class PacketSupport
    {
        private byte[] data;
        private int length;
        private System.Net.IPEndPoint ipEndPoint;

        int port = -1;
        System.Net.IPAddress address;

        /// <summary>
        /// Constructor for the packet
        /// </summary>	
        /// <param name="data">The buffer to store the data</param>	
        /// <param name="data">The length of the data sent</param>	
        /// <param name="length"></param>
        /// <returns>A new packet to receive data of the specified length</returns>	
        public PacketSupport(byte[] data, int length)
        {
            if (length > data.Length)
                throw new ArgumentException("illegal length");

            this.data = data;
            this.length = length;
            ipEndPoint = null;
        }

        /// <summary>
        /// Constructor for the packet
        /// </summary>	
        /// <param name="data">The data to be sent</param>	
        /// <param name="data">The length of the data to be sent</param>	
        /// <param name="data">The IP of the destination point</param>	
        /// <param name="length"></param>
        /// <param name="ipendpoint"></param>
        /// <returns>A new packet with the data, length and ipEndPoint set</returns>
        public PacketSupport(byte[] data, int length, System.Net.IPEndPoint ipendpoint)
        {
            if (length > data.Length)
                throw new ArgumentException("illegal length");

            this.data = data;
            this.length = length;
            ipEndPoint = ipendpoint;
        }

        /// <summary>
        /// Gets and sets the address of the IP
        /// </summary>			
        /// <returns>The IP address</returns>
        public System.Net.IPEndPoint IPEndPoint
        {
            get
            {
                return ipEndPoint;
            }
            set
            {
                ipEndPoint = value;
            }
        }

        /// <summary>
        /// Gets and sets the address
        /// </summary>			
        /// <returns>The int value of the address</returns>
        public System.Net.IPAddress Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                if (ipEndPoint == null)
                {
                    if (Port >= 0 && Port <= 0xFFFF)
                        ipEndPoint = new System.Net.IPEndPoint(value, Port);
                }
                else
                    ipEndPoint.Address = value;
            }
        }

        /// <summary>
        /// Gets and sets the port
        /// </summary>			
        /// <returns>The int value of the port</returns>
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                if (value < 0 || value > 0xFFFF)
                    throw new ArgumentException("Port out of range:" + value);

                port = value;
                if (ipEndPoint == null)
                {
                    if (Address != null)
                        ipEndPoint = new System.Net.IPEndPoint(Address, value);
                }
                else
                    ipEndPoint.Port = value;
            }
        }

        /// <summary>
        /// Gets and sets the length of the data
        /// </summary>			
        /// <returns>The int value of the length</returns>
        public int Length
        {
            get
            {
                return length;
            }
            set
            {
                if (value > data.Length)
                    throw new ArgumentException("illegal length");

                length = value;
            }
        }

        /// <summary>
        /// Gets and sets the byte array that contains the data
        /// </summary>			
        /// <returns>The byte array that contains the data</returns>
        public byte[] Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }
    }


    /*******************************/
    /// <summary>
    /// Support class used to extend System.Net.Sockets.UdpClient class functionality
    /// </summary>
    public class UdpClientSupport : System.Net.Sockets.UdpClient
    {

        public int port = -1;

        public System.Net.IPEndPoint ipEndPoint;

        public String host;


        /// <summary>
        /// Initializes a new instance of the UdpClientSupport class, and binds it to the local port number provided.
        /// </summary>
        /// <param name="port">The local port number from which you intend to communicate.</param>
        public UdpClientSupport(int port)
            : base(port)
        {
            this.port = port;
        }

        /// <summary>
        /// Initializes a new instance of the UdpClientSupport class.
        /// </summary>
        public UdpClientSupport()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UdpClientSupport class,
        /// and binds it to the specified local endpoint.
        /// </summary>
        /// <param name="IP">An IPEndPoint that respresents the local endpoint to which you bind the UDP connection.</param>
        public UdpClientSupport(System.Net.IPEndPoint IP)
            : base(IP)
        {
            ipEndPoint = IP;
            port = ipEndPoint.Port;
        }

        /// <summary>
        /// Initializes a new instance of the UdpClientSupport class,
        /// and and establishes a default remote host.
        /// </summary>
        /// <param name="host">The name of the remote DNS host to which you intend to connect.</param>
        /// <param name="port">The remote port number to which you intend to connect. </param>
        public UdpClientSupport(string host, int port)
            : base(host, port)
        {
            this.host = host;
            this.port = port;
        }

        /// <summary>
        /// Returns a UDP datagram that was sent by a remote host.
        /// </summary>
        /// <param name="tempClient">UDP client instance to use to receive the datagram</param>
        /// <param name="packet">Instance of the recieved datagram packet</param>
        public static void Receive(System.Net.Sockets.UdpClient tempClient, out PacketSupport packet)
        {
            var remoteIpEndPoint =
                new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);

            PacketSupport tempPacket;
            try
            {
                byte[] data_in = tempClient.Receive(ref remoteIpEndPoint);
                tempPacket = new PacketSupport(data_in, data_in.Length) { IPEndPoint = remoteIpEndPoint };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            packet = tempPacket;
        }

        /// <summary>
        /// Sends a UDP datagram to the host at the specified remote endpoint.
        /// </summary>
        /// <param name="tempClient">Client to use as source for sending the datagram</param>
        /// <param name="packet">Packet containing the datagram data to send</param>
        public static void Send(System.Net.Sockets.UdpClient tempClient, PacketSupport packet)
        {
            tempClient.Send(packet.Data, packet.Length, packet.IPEndPoint);
        }


        /// <summary>
        /// Gets and sets the address of the IP
        /// </summary>			
        /// <returns>The IP address</returns>
        public System.Net.IPEndPoint IPEndPoint
        {
            get
            {
                return ipEndPoint;
            }
            set
            {
                ipEndPoint = value;
            }
        }

        /// <summary>
        /// Gets and sets the port
        /// </summary>			
        /// <returns>The int value of the port</returns>
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                if (value < 0 || value > 0xFFFF)
                    throw new ArgumentException("Port out of range:" + value);

                port = value;
            }
        }


        /// <summary>
        /// Gets the address of the IP
        /// </summary>			
        /// <returns>The IP address</returns>
        public System.Net.IPAddress getIPEndPointAddress()
        {
            return ipEndPoint != null ? ipEndPoint.Address : null;
        }
    }

    ///*******************************/
    ///// <summary>
    ///// This class provides functionality not found in .NET collection-related interfaces.
    ///// </summary>
    public class ICollectionSupport
    {

        public static T[] ToArray<T>(System.Collections.ICollection c)
        {
            var index = 0;

            var objs = new T[c.Count];

            var e = c.GetEnumerator();

            while (e.MoveNext())
                objs[index++] = (T)e.Current;


            return objs;
        }

    }


    /*******************************/
    /// <summary>
    /// SupportClass for the HashSet class.
    /// </summary>
    [Serializable]
    public class HashSetSupport : System.Collections.ArrayList, SetSupport
    {
        public HashSetSupport()
        {
        }

        public HashSetSupport(System.Collections.ICollection c)
        {
            AddAll(c);
        }

        public HashSetSupport(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Adds a new element to the ArrayList if it is not already present.
        /// </summary>		
        /// <param name="obj">Element to insert to the ArrayList.</param>
        /// <returns>Returns true if the new element was inserted, false otherwise.</returns>
        new public virtual bool Add(Object obj)
        {
            bool inserted;

            if ((inserted = Contains(obj)) == false)
            {
                base.Add(obj);
            }

            return !inserted;
        }

        /// <summary>
        /// Adds all the elements of the specified collection that are not present to the list.
        /// </summary>
        /// <param name="c">Collection where the new elements will be added</param>
        /// <returns>Returns true if at least one element was added, false otherwise.</returns>
        public bool AddAll(System.Collections.ICollection c)
        {
            var e = new System.Collections.ArrayList(c).GetEnumerator();
            var added = false;

            while (e.MoveNext())
            {
                if (Add(e.Current))
                    added = true;
            }

            return added;
        }

        /// <summary>
        /// Returns a copy of the HashSet instance.
        /// </summary>		
        /// <returns>Returns a shallow copy of the current HashSet.</returns>
        public override Object Clone()
        {
            return MemberwiseClone();
        }
    }


    /*******************************/
    /// <summary>
    /// Represents a collection ob objects that contains no duplicate elements.
    /// </summary>	
    public interface SetSupport : System.Collections.IList
    {
        /// <summary>
        /// Adds a new element to the Collection if it is not already present.
        /// </summary>
        /// <param name="obj">The object to add to the collection.</param>
        /// <returns>Returns true if the object was added to the collection, otherwise false.</returns>
        new bool Add(Object obj);

        /// <summary>
        /// Adds all the elements of the specified collection to the Set.
        /// </summary>
        /// <param name="c">Collection of objects to add.</param>
        /// <returns>true</returns>
        bool AddAll(System.Collections.ICollection c);
    }


    /*******************************/
    /// <summary>
    /// Support class used to handle threads
    /// </summary>
    public class ThreadClass : IThreadRunnable
    {
        /// <summary>
        /// The instance of System.Threading.Thread
        /// </summary>
        private System.Threading.Thread threadField;

        /// <summary>
        /// Initializes a new instance of the ThreadClass class
        /// </summary>
        public ThreadClass()
        {
            threadField = new System.Threading.Thread(Run);
        }

        /// <summary>
        /// Initializes a new instance of the Thread class.
        /// </summary>
        /// <param name="Name">The name of the thread</param>
        public ThreadClass(string Name)
        {
            threadField = new System.Threading.Thread(Run);
            this.Name = Name;
        }

        /// <summary>
        /// Initializes a new instance of the Thread class.
        /// </summary>
        /// <param name="Start">A ThreadStart delegate that references the methods to be invoked when this thread begins executing</param>
        public ThreadClass(System.Threading.ThreadStart Start)
        {
            threadField = new System.Threading.Thread(Start);
        }

        /// <summary>
        /// Initializes a new instance of the Thread class.
        /// </summary>
        /// <param name="Start">A ThreadStart delegate that references the methods to be invoked when this thread begins executing</param>
        /// <param name="Name">The name of the thread</param>
        public ThreadClass(System.Threading.ThreadStart Start, string Name)
        {
            threadField = new System.Threading.Thread(Start);
            this.Name = Name;
        }

        /// <summary>
        /// This method has no functionality unless the method is overridden
        /// </summary>
        public virtual void Run()
        {
        }

        /// <summary>
        /// Causes the operating system to change the state of the current thread instance to ThreadState.Running
        /// </summary>
        public virtual void Start()
        {
            threadField.Start();
        }

        /// <summary>
        /// Interrupts a thread that is in the WaitSleepJoin thread state
        /// </summary>
        public virtual void Interrupt()
        {
            threadField.Interrupt();
        }

        /// <summary>
        /// Gets the current thread instance
        /// </summary>
        public System.Threading.Thread Instance
        {
            get
            {
                return threadField;
            }
            set
            {
                threadField = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the thread
        /// </summary>
        public string Name
        {
            get
            {
                return threadField.Name;
            }
            set
            {
                if (threadField.Name == null)
                    threadField.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the scheduling priority of a thread
        /// </summary>
        public System.Threading.ThreadPriority Priority
        {
            get
            {
                return threadField.Priority;
            }
            set
            {
                threadField.Priority = value;
            }
        }

        /// <summary>
        /// Gets a value indicating the execution status of the current thread
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return threadField.IsAlive;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a thread is a background thread.
        /// </summary>
        public bool IsBackground
        {
            get
            {
                return threadField.IsBackground;
            }
            set
            {
                threadField.IsBackground = value;
            }
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates
        /// </summary>
        public void Join()
        {
            threadField.Join();
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates or the specified time elapses
        /// </summary>
        /// <param name="MiliSeconds">Time of wait in milliseconds</param>
        public void Join(long MiliSeconds)
        {
            lock (this)
            {
                threadField.Join(new TimeSpan(MiliSeconds * 10000));
            }
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates or the specified time elapses
        /// </summary>
        /// <param name="MiliSeconds">Time of wait in milliseconds</param>
        /// <param name="NanoSeconds">Time of wait in nanoseconds</param>
        public void Join(long MiliSeconds, int NanoSeconds)
        {
            lock (this)
            {
                threadField.Join(new TimeSpan(MiliSeconds * 10000 + NanoSeconds * 100));
            }
        }

        readonly object suspendResume = new object();

        /// <summary>
        /// Resumes a thread that has been suspended
        /// </summary>
        public void Resume()
        {
            lock (suspendResume)
            {
                System.Threading.Monitor.Pulse(suspendResume);
            }
            //threadField.Resume();
        }

        /// <summary>
        /// Suspends the thread, if the thread is already suspended it has no effect
        /// </summary>
        public void Suspend()
        {
            lock (suspendResume)
            {
                System.Threading.Monitor.Wait(suspendResume);
            }
            //threadField.Suspend();
        }

        /// <summary>
        /// Raises a ThreadAbortException in the thread on which it is invoked, 
        /// to begin the process of terminating the thread. Calling this method 
        /// usually terminates the thread
        /// </summary>
        public void Abort()
        {
            threadField.Abort();
        }

        /// <summary>
        /// Raises a ThreadAbortException in the thread on which it is invoked, 
        /// to begin the process of terminating the thread while also providing
        /// exception information about the thread termination. 
        /// Calling this method usually terminates the thread.
        /// </summary>
        /// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted</param>
        public void Abort(Object stateInfo)
        {
            lock (this)
            {
                threadField.Abort(stateInfo);
            }
        }



        /// <summary>
        /// Obtain a String that represents the current Object
        /// </summary>
        /// <returns>A String that represents the current Object</returns>
        public override string ToString()
        {
            return "Thread[" + Name + "," + Priority + "," + "" + "]";
        }

        /// <summary>
        /// Gets the currently running thread
        /// </summary>
        /// <returns>The currently running thread</returns>
        public static ThreadClass Current()
        {
            var CurrentThread = new ThreadClass { Instance = System.Threading.Thread.CurrentThread };
            return CurrentThread;
        }
    }


    /*******************************/

    public static System.Xml.XmlDocument ParseDocument(System.Xml.XmlDocument document, XmlSourceSupport source)
    {
        if (source.Characters != null)
        {
            document.Load(source.Characters.BaseStream);
            return (System.Xml.XmlDocument)document.Clone();
        }
        if (source.Bytes != null)
        {
            document.Load(source.Bytes);
            return (System.Xml.XmlDocument)document.Clone();
        }
        if (source.Uri != null)
        {
            document.Load(source.Uri);
            return (System.Xml.XmlDocument)document.Clone();
        }
        throw new System.Xml.XmlException("The XmlSource class can't be null");
    }

    /*******************************/
    /// <summary>
    /// Provides support for DateFormat
    /// </summary>
    public class DateTimeFormatManager
    {
        static public DateTimeFormatHashTable manager = new DateTimeFormatHashTable();

        /// <summary>
        /// Hashtable class to provide functionality for dateformat properties
        /// </summary>
        public class DateTimeFormatHashTable : System.Collections.Hashtable
        {
            /// <summary>
            /// Sets the format for datetime.
            /// </summary>
            /// <param name="format">DateTimeFormat instance to set the pattern</param>
            /// <param name="newPattern">A string with the pattern format</param>
            public void SetDateFormatPattern(System.Globalization.DateTimeFormatInfo format, string newPattern)
            {
                if (this[format] != null)
                    ((DateTimeFormatProperties)this[format]).DateFormatPattern = newPattern;
                else
                {
                    var tempProps = new DateTimeFormatProperties { DateFormatPattern = newPattern };
                    Add(format, tempProps);
                }
            }

            /// <summary>
            /// Gets the current format pattern of the DateTimeFormat instance
            /// </summary>
            /// <param name="format">The DateTimeFormat instance which the value will be obtained</param>
            /// <returns>The string representing the current datetimeformat pattern</returns>
            public string GetDateFormatPattern(System.Globalization.DateTimeFormatInfo format)
            {
                if (this[format] == null)
                    return "d-MMM-yy";
                return ((DateTimeFormatProperties)this[format]).DateFormatPattern;
            }

            /// <summary>
            /// Sets the datetimeformat pattern to the giving format
            /// </summary>
            /// <param name="format">The datetimeformat instance to set</param>
            /// <param name="newPattern">The new datetimeformat pattern</param>
            public void SetTimeFormatPattern(System.Globalization.DateTimeFormatInfo format, string newPattern)
            {
                if (this[format] != null)
                    ((DateTimeFormatProperties)this[format]).TimeFormatPattern = newPattern;
                else
                {
                    var tempProps = new DateTimeFormatProperties { TimeFormatPattern = newPattern };
                    Add(format, tempProps);
                }
            }

            /// <summary>
            /// Gets the current format pattern of the DateTimeFormat instance
            /// </summary>
            /// <param name="format">The DateTimeFormat instance which the value will be obtained</param>
            /// <returns>The string representing the current datetimeformat pattern</returns>
            public string GetTimeFormatPattern(System.Globalization.DateTimeFormatInfo format)
            {
                if (this[format] == null)
                    return "h:mm:ss tt";
                return ((DateTimeFormatProperties)this[format]).TimeFormatPattern;
            }

            /// <summary>
            /// Internal class to provides the DateFormat and TimeFormat pattern properties on .NET
            /// </summary>
            class DateTimeFormatProperties
            {
                public string DateFormatPattern = "d-MMM-yy";
                public string TimeFormatPattern = "h:mm:ss tt";
            }
        }
    }
    /*******************************/
    /// <summary>
    /// Gets the DateTimeFormat instance and date instance to obtain the date with the format passed
    /// </summary>
    /// <param name="format">The DateTimeFormat to obtain the time and date pattern</param>
    /// <param name="date">The date instance used to get the date</param>
    /// <returns>A string representing the date with the time and date patterns</returns>
    public static string FormatDateTime(System.Globalization.DateTimeFormatInfo format, DateTime date)
    {
        var timePattern = DateTimeFormatManager.manager.GetTimeFormatPattern(format);
        var datePattern = DateTimeFormatManager.manager.GetDateFormatPattern(format);
        return date.ToString(datePattern + " " + timePattern, format);
    }

    /*******************************/
    /// <summary>
    /// This class manages different features for calendars.
    /// The different calendars are internally managed using a hashtable structure.
    /// </summary>
    public class CalendarManager
    {
        /// <summary>
        /// Field used to get or set the year.
        /// </summary>
        public const int YEAR = 1;

        /// <summary>
        /// Field used to get or set the month.
        /// </summary>
        public const int MONTH = 2;

        /// <summary>
        /// Field used to get or set the day of the month.
        /// </summary>
        public const int DATE = 5;

        /// <summary>
        /// Field used to get or set the hour of the morning or afternoon.
        /// </summary>
        public const int HOUR = 10;

        /// <summary>
        /// Field used to get or set the minute within the hour.
        /// </summary>
        public const int MINUTE = 12;

        /// <summary>
        /// Field used to get or set the second within the minute.
        /// </summary>
        public const int SECOND = 13;

        /// <summary>
        /// Field used to get or set the millisecond within the second.
        /// </summary>
        public const int MILLISECOND = 14;

        /// <summary>
        /// Field used to get or set the day of the year.
        /// </summary>
        public const int DAY_OF_YEAR = 4;

        /// <summary>
        /// Field used to get or set the day of the month.
        /// </summary>
        public const int DAY_OF_MONTH = 6;

        /// <summary>
        /// Field used to get or set the day of the week.
        /// </summary>
        public const int DAY_OF_WEEK = 7;

        /// <summary>
        /// Field used to get or set the hour of the day.
        /// </summary>
        public const int HOUR_OF_DAY = 11;

        /// <summary>
        /// Field used to get or set whether the HOUR is before or after noon.
        /// </summary>
        public const int AM_PM = 9;

        /// <summary>
        /// Field used to get or set the value of the AM_PM field which indicates the period of the day from midnight to just before noon.
        /// </summary>
        public const int AM = 0;

        /// <summary>
        /// Field used to get or set the value of the AM_PM field which indicates the period of the day from noon to just before midnight.
        /// </summary>
        public const int PM = 1;

        /// <summary>
        /// The hashtable that contains the calendars and its properties.
        /// </summary>
        static public CalendarHashTable manager = new CalendarHashTable();

        /// <summary>
        /// Internal class that inherits from HashTable to manage the different calendars.
        /// This structure will contain an instance of System.Globalization.Calendar that represents 
        /// a type of calendar and its properties (represented by an instance of CalendarProperties 
        /// class).
        /// </summary>
        public class CalendarHashTable : System.Collections.Hashtable
        {
            /// <summary>
            /// Gets the calendar current date and time.
            /// </summary>
            /// <param name="calendar">The calendar to get its current date and time.</param>
            /// <returns>A System.DateTime value that indicates the current date and time for the 
            /// calendar given.</returns>
            public DateTime GetDateTime(System.Globalization.Calendar calendar)
            {
                if (this[calendar] != null)
                    return ((CalendarProperties)this[calendar]).dateTime;
                var tempProps = new CalendarProperties { dateTime = DateTime.Now };
                Add(calendar, tempProps);
                return GetDateTime(calendar);
            }

            /// <summary>
            /// Sets the specified System.DateTime value to the specified calendar.
            /// </summary>
            /// <param name="calendar">The calendar to set its date.</param>
            /// <param name="date">The System.DateTime value to set to the calendar.</param>
            public void SetDateTime(System.Globalization.Calendar calendar, DateTime date)
            {
                if (this[calendar] != null)
                {
                    ((CalendarProperties)this[calendar]).dateTime = date;
                }
                else
                {
                    var tempProps = new CalendarProperties { dateTime = date };
                    Add(calendar, tempProps);
                }
            }

            /// <summary>
            /// Sets the corresponding field in an specified calendar with the value given.
            /// If the specified calendar does not have exist in the hash table, it creates a 
            /// new instance of the calendar with the current date and time and then assings it 
            /// the new specified value.
            /// </summary>
            /// <param name="calendar">The calendar to set its date or time.</param>
            /// <param name="field">One of the fields that composes a date/time.</param>
            /// <param name="fieldValue">The value to be set.</param>
            public void Set(System.Globalization.Calendar calendar, int field, int fieldValue)
            {
                if (this[calendar] != null)
                {
                    System.DateTime tempDate = ((CalendarProperties)this[calendar]).dateTime;
                    switch (field)
                    {
                        case DATE:
                            tempDate = tempDate.AddDays(fieldValue - tempDate.Day);
                            break;
                        case HOUR:
                            tempDate = tempDate.AddHours(fieldValue - tempDate.Hour);
                            break;
                        case MILLISECOND:
                            tempDate = tempDate.AddMilliseconds(fieldValue - tempDate.Millisecond);
                            break;
                        case MINUTE:
                            tempDate = tempDate.AddMinutes(fieldValue - tempDate.Minute);
                            break;
                        case MONTH:
                            //Month value is 0-based. e.g., 0 for January
                            tempDate = tempDate.AddMonths((fieldValue + 1) - tempDate.Month);
                            break;
                        case SECOND:
                            tempDate = tempDate.AddSeconds(fieldValue - tempDate.Second);
                            break;
                        case YEAR:
                            tempDate = tempDate.AddYears(fieldValue - tempDate.Year);
                            break;
                        case DAY_OF_MONTH:
                            tempDate = tempDate.AddDays(fieldValue - tempDate.Day);
                            break;
                        case DAY_OF_WEEK:
                            tempDate = tempDate.AddDays((fieldValue - 1) - (int)tempDate.DayOfWeek);
                            break;
                        case DAY_OF_YEAR:
                            tempDate = tempDate.AddDays(fieldValue - tempDate.DayOfYear);
                            break;
                        case HOUR_OF_DAY:
                            tempDate = tempDate.AddHours(fieldValue - tempDate.Hour);
                            break;

                        default:
                            break;
                    }
                    ((CalendarProperties)this[calendar]).dateTime = tempDate;
                }
                else
                {
                    var tempProps = new CalendarProperties { dateTime = DateTime.Now };
                    Add(calendar, tempProps);
                    Set(calendar, field, fieldValue);
                }
            }

            /// <summary>
            /// Sets the corresponding date (day, month and year) to the calendar specified.
            /// If the calendar does not exist in the hash table, it creates a new instance and sets 
            /// its values.
            /// </summary>
            /// <param name="calendar">The calendar to set its date.</param>
            /// <param name="year">Integer value that represent the year.</param>
            /// <param name="month">Integer value that represent the month.</param>
            /// <param name="day">Integer value that represent the day.</param>
            public void Set(System.Globalization.Calendar calendar, int year, int month, int day)
            {
                if (this[calendar] != null)
                {
                    Set(calendar, YEAR, year);
                    Set(calendar, MONTH, month);
                    Set(calendar, DATE, day);
                }
                else
                {
                    var tempProps = new CalendarProperties { dateTime = DateTime.Now };
                    Add(calendar, tempProps);
                    Set(calendar, year, month, day);
                }
            }

            /// <summary>
            /// Sets the corresponding date (day, month and year) and hour (hour and minute) 
            /// to the calendar specified.
            /// If the calendar does not exist in the hash table, it creates a new instance and sets 
            /// its values.
            /// </summary>
            /// <param name="calendar">The calendar to set its date and time.</param>
            /// <param name="year">Integer value that represent the year.</param>
            /// <param name="month">Integer value that represent the month.</param>
            /// <param name="day">Integer value that represent the day.</param>
            /// <param name="hour">Integer value that represent the hour.</param>
            /// <param name="minute">Integer value that represent the minutes.</param>
            public void Set(System.Globalization.Calendar calendar, int year, int month, int day, int hour, int minute)
            {
                if (this[calendar] != null)
                {
                    Set(calendar, YEAR, year);
                    Set(calendar, MONTH, month);
                    Set(calendar, DATE, day);
                    Set(calendar, HOUR, hour);
                    Set(calendar, MINUTE, minute);
                }
                else
                {
                    var tempProps = new CalendarProperties { dateTime = DateTime.Now };
                    Add(calendar, tempProps);
                    Set(calendar, year, month, day, hour, minute);
                }
            }

            /// <summary>
            /// Sets the corresponding date (day, month and year) and hour (hour, minute and second) 
            /// to the calendar specified.
            /// If the calendar does not exist in the hash table, it creates a new instance and sets 
            /// its values.
            /// </summary>
            /// <param name="calendar">The calendar to set its date and time.</param>
            /// <param name="year">Integer value that represent the year.</param>
            /// <param name="month">Integer value that represent the month.</param>
            /// <param name="day">Integer value that represent the day.</param>
            /// <param name="hour">Integer value that represent the hour.</param>
            /// <param name="minute">Integer value that represent the minutes.</param>
            /// <param name="second">Integer value that represent the seconds.</param>
            public void Set(System.Globalization.Calendar calendar, int year, int month, int day, int hour, int minute, int second)
            {
                if (this[calendar] != null)
                {
                    Set(calendar, YEAR, year);
                    Set(calendar, MONTH, month);
                    Set(calendar, DATE, day);
                    Set(calendar, HOUR, hour);
                    Set(calendar, MINUTE, minute);
                    Set(calendar, SECOND, second);
                }
                else
                {
                    var tempProps = new CalendarProperties { dateTime = DateTime.Now };
                    Add(calendar, tempProps);
                    Set(calendar, year, month, day, hour, minute, second);
                }
            }

            /// <summary>
            /// Gets the value represented by the field specified.
            /// </summary>
            /// <param name="calendar">The calendar to get its date or time.</param>
            /// <param name="field">One of the field that composes a date/time.</param>
            /// <returns>The integer value for the field given.</returns>
            public int Get(System.Globalization.Calendar calendar, int field)
            {
                if (this[calendar] != null)
                {
                    int tempHour;
                    switch (field)
                    {
                        case DATE:
                            return ((CalendarProperties)this[calendar]).dateTime.Day;
                        case HOUR:
                            tempHour = ((CalendarProperties)this[calendar]).dateTime.Hour;
                            return tempHour > 12 ? tempHour - 12 : tempHour;
                        case MILLISECOND:
                            return ((CalendarProperties)this[calendar]).dateTime.Millisecond;
                        case MINUTE:
                            return ((CalendarProperties)this[calendar]).dateTime.Minute;
                        case MONTH:
                            //Month value is 0-based. e.g., 0 for January
                            return ((CalendarProperties)this[calendar]).dateTime.Month - 1;
                        case SECOND:
                            return ((CalendarProperties)this[calendar]).dateTime.Second;
                        case YEAR:
                            return ((CalendarProperties)this[calendar]).dateTime.Year;
                        case DAY_OF_MONTH:
                            return ((CalendarProperties)this[calendar]).dateTime.Day;
                        case DAY_OF_YEAR:
                            return ((CalendarProperties)this[calendar]).dateTime.DayOfYear;
                        case DAY_OF_WEEK:
                            return (int)(((CalendarProperties)this[calendar]).dateTime.DayOfWeek) + 1;
                        case HOUR_OF_DAY:
                            return ((CalendarProperties)this[calendar]).dateTime.Hour;
                        case AM_PM:
                            tempHour = ((CalendarProperties)this[calendar]).dateTime.Hour;
                            return tempHour > 12 ? PM : AM;

                        default:
                            return 0;
                    }
                }
                var tempProps = new CalendarProperties { dateTime = DateTime.Now };
                Add(calendar, tempProps);
                return Get(calendar, field);
            }

            /// <summary>
            /// Sets the time in the specified calendar with the long value.
            /// </summary>
            /// <param name="calendar">The calendar to set its date and time.</param>
            /// <param name="milliseconds">A long value that indicates the milliseconds to be set to 
            /// the hour for the calendar.</param>
            public void SetTimeInMilliseconds(System.Globalization.Calendar calendar, long milliseconds)
            {
                if (this[calendar] != null)
                {
                    ((CalendarProperties)this[calendar]).dateTime = new DateTime(milliseconds);
                }
                else
                {
                    var tempProps = new CalendarProperties
                    {
                        dateTime =
                            new DateTime(TimeSpan.TicksPerMillisecond * milliseconds)
                    };
                    Add(calendar, tempProps);
                }
            }

            /// <summary>
            /// Gets what the first day of the week is; e.g., Sunday in US, Monday in France.
            /// </summary>
            /// <param name="calendar">The calendar to get its first day of the week.</param>
            /// <returns>A System.DayOfWeek value indicating the first day of the week.</returns>
            public DayOfWeek GetFirstDayOfWeek(System.Globalization.Calendar calendar)
            {
                if (this[calendar] != null)
                {
                    if (((CalendarProperties)this[calendar]).dateTimeFormat == null)
                    {
                        ((CalendarProperties)this[calendar]).dateTimeFormat = new System.Globalization.DateTimeFormatInfo
                        {
                            FirstDayOfWeek = DayOfWeek.Sunday
                        };
                    }
                    return ((CalendarProperties)this[calendar]).dateTimeFormat.FirstDayOfWeek;
                }
                var tempProps = new CalendarProperties
                {
                    dateTime = DateTime.Now,
                    dateTimeFormat = new System.Globalization.DateTimeFormatInfo
                    {
                        FirstDayOfWeek = DayOfWeek.Sunday
                    }
                };
                Add(calendar, tempProps);
                return GetFirstDayOfWeek(calendar);
            }

            /// <summary>
            /// Sets what the first day of the week is; e.g., Sunday in US, Monday in France.
            /// </summary>
            /// <param name="calendar">The calendar to set its first day of the week.</param>
            /// <param name="firstDayOfWeek">A System.DayOfWeek value indicating the first day of the week
            /// to be set.</param>
            public void SetFirstDayOfWeek(System.Globalization.Calendar calendar, DayOfWeek firstDayOfWeek)
            {
                if (this[calendar] != null)
                {
                    if (((CalendarProperties)this[calendar]).dateTimeFormat == null)
                        ((CalendarProperties)this[calendar]).dateTimeFormat = new System.Globalization.DateTimeFormatInfo();

                    ((CalendarProperties)this[calendar]).dateTimeFormat.FirstDayOfWeek = firstDayOfWeek;
                }
                else
                {
                    var tempProps = new CalendarProperties
                    {
                        dateTime = DateTime.Now,
                        dateTimeFormat = new System.Globalization.DateTimeFormatInfo()
                    };
                    Add(calendar, tempProps);
                    SetFirstDayOfWeek(calendar, firstDayOfWeek);
                }
            }

            /// <summary>
            /// Removes the specified calendar from the hash table.
            /// </summary>
            /// <param name="calendar">The calendar to be removed.</param>
            public void Clear(System.Globalization.Calendar calendar)
            {
                if (this[calendar] != null)
                    Remove(calendar);
            }

            /// <summary>
            /// Removes the specified field from the calendar given.
            /// If the field does not exists in the calendar, the calendar is removed from the table.
            /// </summary>
            /// <param name="calendar">The calendar to remove the value from.</param>
            /// <param name="field">The field to be removed from the calendar.</param>
            public void Clear(System.Globalization.Calendar calendar, int field)
            {
                if (this[calendar] != null)
                    Set(calendar, field, 0);
            }

            /// <summary>
            /// Internal class that represents the properties of a calendar instance.
            /// </summary>
            class CalendarProperties
            {
                /// <summary>
                /// The date and time of a calendar.
                /// </summary>
                public DateTime dateTime;

                /// <summary>
                /// The format for the date and time in a calendar.
                /// </summary>
                public System.Globalization.DateTimeFormatInfo dateTimeFormat;
            }
        }
    }
    /*******************************/
    /// <summary>
    /// This method returns the literal value received
    /// </summary>
    /// <param name="literal">The literal to return</param>
    /// <returns>The received value</returns>
    public static long Identity(long literal)
    {
        return literal;
    }

    /// <summary>
    /// This method returns the literal value received
    /// </summary>
    /// <param name="literal">The literal to return</param>
    /// <returns>The received value</returns>
    public static ulong Identity(ulong literal)
    {
        return literal;
    }

    /// <summary>
    /// This method returns the literal value received
    /// </summary>
    /// <param name="literal">The literal to return</param>
    /// <returns>The received value</returns>
    public static float Identity(float literal)
    {
        return literal;
    }

    /// <summary>
    /// This method returns the literal value received
    /// </summary>
    /// <param name="literal">The literal to return</param>
    /// <returns>The received value</returns>
    public static double Identity(double literal)
    {
        return literal;
    }

    public static int BigDecimal_Scale(decimal d)
    {
        int val = 0;
        while (Math.Truncate(d) != d)
        {
            d = d * 10;
            val++;
        }
        return val;
    }


    public static long BigDecimal_UnScaledValue(decimal d)
    {
        while (Math.Truncate(d) != d)
        {
            d = d * 10;
        }
        return (long)Math.Truncate(d);
    }
}
