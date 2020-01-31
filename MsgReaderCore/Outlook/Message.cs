﻿//
// Message.cs
//
// Author: Kees van Spelde <sicos2002@hotmail.com>
//
// Copyright (c) 2013-2019 Magic-Sessions. (www.magic-sessions.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MsgReader.Exceptions;
using MsgReader.Helpers;
using MsgReader.Localization;
using MsgReader.Mime.Header;
using OpenMcdf;
// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace MsgReader.Outlook
{
    #region Enum MessageType
    /// <summary>
    /// The message types
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// The message type is unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// The message is a normal E-mail
        /// </summary>
        Email,

        /// <summary>
        /// Non-delivery report for a standard E-mail (REPORT.IPM.NOTE.NDR)
        /// </summary>
        EmailNonDeliveryReport,

        /// <summary>
        /// Delivery receipt for a standard E-mail (REPORT.IPM.NOTE.DR)
        /// </summary>
        EmailDeliveryReport,

        /// <summary>
        /// Delivery receipt for a delayed E-mail (REPORT.IPM.NOTE.DELAYED)
        /// </summary>
        EmailDelayedDeliveryReport,

        /// <summary>
        /// Read receipt for a standard E-mail (REPORT.IPM.NOTE.IPNRN)
        /// </summary>
        EmailReadReceipt,

        /// <summary>
        /// Non-read receipt for a standard E-mail (REPORT.IPM.NOTE.IPNNRN)
        /// </summary>
        EmailNonReadReceipt,

        /// <summary>
        /// The message in an E-mail that is encrypted and can also be signed (IPM.Note.SMIME)
        /// </summary>
        EmailEncryptedAndMaybeSigned,

        /// <summary>
        /// Non-delivery report for a Secure MIME (S/MIME) encrypted and opaque-signed E-mail (REPORT.IPM.NOTE.SMIME.NDR)
        /// </summary>
        EmailEncryptedAndMaybeSignedNonDelivery,

        /// <summary>
        /// Delivery report for a Secure MIME (S/MIME) encrypted and opaque-signed E-mail (REPORT.IPM.NOTE.SMIME.DR)
        /// </summary>
        EmailEncryptedAndMaybeSignedDelivery,
        
        /// <summary>
        /// The message is an E-mail that is clear signed (IPM.Note.SMIME.MultipartSigned)
        /// </summary>
        EmailClearSigned,

        /// <summary>
        /// The message is a secure read receipt for an E-mail (IPM.Note.Receipt.SMIME)
        /// </summary>
        EmailClearSignedReadReceipt,

        /// <summary>
        /// Non-delivery report for an S/MIME clear-signed E-mail (REPORT.IPM.NOTE.SMIME.MULTIPARTSIGNED.NDR)
        /// </summary>
        EmailClearSignedNonDelivery,

        /// <summary>
        /// Delivery receipt for an S/MIME clear-signed E-mail (REPORT.IPM.NOTE.SMIME.MULTIPARTSIGNED.DR)
        /// </summary>
        EmailClearSignedDelivery,

        /// <summary>
        /// The message is an E-mail that is generared signed (IPM.Note.BMA.Stub)
        /// </summary>
        EmailBmaStub,

        /// <summary>
        /// The message is a short message service (IPM.Note.Mobile.SMS)
        /// </summary>
        EmailSms,

        /// <summary>
        /// The message is a Microsoft template (IPM.Note.Rules.OofTemplate.Microsoft)
        /// </summary>
        EmailTemplateMicrosoft,
        
        /// <summary>
        /// The message is an appointment (IPM.Appointment)
        /// </summary>
        Appointment,

        /// <summary>
        /// The message is a notification for an appointment (IPM.Notification.Meeting)
        /// </summary>
        AppointmentNotification,

        /// <summary>
        /// The message is a schedule for an appointment (IPM.Schedule.Meeting)
        /// </summary>
        AppointmentSchedule,

        /// <summary>
        /// The message is a request for an appointment (IPM.Schedule.Meeting.Request)
        /// </summary>
        AppointmentRequest,

        /// <summary>
        /// The message is a request for an appointment (REPORT.IPM.SCHEDULE.MEETING.REQUEST.NDR)
        /// </summary>
        AppointmentRequestNonDelivery,

        /// <summary>
        /// The message is a response to an appointment (IPM.Schedule.Response)
        /// </summary>
        AppointmentResponse,

        /// <summary>
        /// The message is a positive response to an appointment (IPM.Schedule.Resp.Pos)
        /// </summary>
        AppointmentResponsePositive,

        /// <summary>
        /// Non-delivery report for a positive meeting response (accept) (REPORT.IPM.SCHEDULE.MEETING.RESP.POS.NDR)
        /// </summary>
        AppointmentResponsePositiveNonDelivery,

        /// <summary>
        /// The message is a negative response to an appointment (IPM.Schedule.Resp.Neg)
        /// </summary>
        AppointmentResponseNegative,

        /// <summary>
        /// Non-delivery report for a negative meeting response (declinet) (REPORT.IPM.SCHEDULE.MEETING.RESP.NEG.NDR)
        /// </summary>
        AppointmentResponseNegativeNonDelivery,

        /// <summary>
        /// The message is a response to tentatively accept the meeting request (IPM.Schedule.Meeting.Resp.Tent)
        /// </summary>
        AppointmentResponseTentative,
        
        /// <summary>
        /// Non-delivery report for a Tentative meeting response (REPORT.IPM.SCHEDULE.MEETING.RESP.TENT.NDR)
        /// </summary>
        AppointmentResponseTentativeNonDelivery,

        /// <summary>
        /// The message is a cancelation an appointment (IPM.Schedule.Meeting.Canceled)
        /// </summary>
        AppointmentResponseCanceled,

        /// <summary>
        /// Non-delivery report for a cancelled meeting notification (REPORT.IPM.SCHEDULE.MEETING.CANCELED.NDR)
        /// </summary>
        AppointmentResponseCanceledNonDelivery,

        /// <summary>
        /// The message is a contact card (IPM.Contact)
        /// </summary>
        Contact,

        /// <summary>
        /// The message is a task (IPM.Task)
        /// </summary>
        Task,

        /// <summary>
        /// The message is a task request accept (IPM.TaskRequest.Accept)
        /// </summary>
        TaskRequestAccept,

        /// <summary>
        /// The message is a task request decline (IPM.TaskRequest.Decline)
        /// </summary>
        TaskRequestDecline,

        /// <summary>
        /// The message is a task request update (IPM.TaskRequest.Update)
        /// </summary>
        TaskRequestUpdate,

        /// <summary>
        /// The message is a sticky note (IPM.StickyNote)
        /// </summary>
        StickyNote,

        /// <summary>
        /// The message is Cisco Unity Voice message (IPM.Note.Custom.Cisco.Unity.Voice)
        /// </summary>
        CiscoUnityVoiceMessage,

        /// <summary>
        /// IPM.NOTE.RIGHTFAX.ADV
        /// </summary>
        RightFaxAdv,

        /// <summary>
        /// The message is Skype for Business missed message (IPM.Note.Microsoft.Missed)
        /// </summary>
        SkypeForBusinessMissedMessage,

        /// <summary>
        /// The message is a Skype for Business conversation (IPM.Note.Microsoft.Conversation)
        /// </summary>
        SkypeForBusinessConversation
    }
    #endregion

    #region Enum MessageImportance
    /// <summary>
    /// The importancy of the message
    /// </summary>
    public enum MessageImportance
    {
        /// <summary>
        /// Low
        /// </summary>
        Low = 0,

        /// <summary>
        /// Normal
        /// </summary>
        Normal = 1,

        /// <summary>
        /// High
        /// </summary>
        High = 2
    }
    #endregion

    public partial class Storage
    {
        /// <summary>
        /// Class represent a MSG object
        /// </summary>
        public class Message : Storage
        {
            #region Fields
            /// <summary>
            /// The name of the <see cref="CFStorage"/> stream that contains this message
            /// </summary>
            internal string StorageName { get; }

            /// <summary>
            /// Contains the <see cref="MessageType"/> of this Message
            /// </summary>
            private MessageType _type = MessageType.Unknown;

            /// <summary>
            /// contains the name of the <see cref="Storage.Message"/> file
            /// </summary>
            private string _fileName;

            /// <summary>
            /// Contains the date and time when the message was created or null
            /// when not available
            /// </summary>
            private DateTime? _creationTime;

            /// <summary>
            /// Contains the name of the last user (or creator) that has changed the Message object or
            /// null when not available
            /// </summary>
            private string _lastModifierName;

            /// <summary>
            /// Contains the date and time when the message was created or null
            /// when not available
            /// </summary>
            private DateTime? _lastModificationTime;

            /// <summary>
            /// contains all the <see cref="Storage.Recipient"/> objects
            /// </summary>
            private readonly List<Recipient> _recipients = new List<Recipient>();

            /// <summary>
            /// Contains an URL to the help page of a mailing list
            /// </summary>
            private string _mailingListHelp;

            /// <summary>
            /// Contains an URL to the subscribe page of a mailing list
            /// </summary>
            private string _mailingListSubscribe;

            /// <summary>
            /// Contains an URL to the unsubscribe page of a mailing list
            /// </summary>
            private string _mailingListUnsubscribe;

            /// <summary>
            /// Contains the date/time in UTC format when the <see cref="Storage.Message"/> object has been sent,
            /// null when not available
            /// </summary>
            private DateTime? _sentOn;

            /// <summary>
            /// Contains the date/time in UTC format when the <see cref="Storage.Message"/> object has been received,
            /// null when not available
            /// </summary>
            private DateTime? _receivedOn;

            /// <summary>
            /// Contains the <see cref="MessageImportance"/> of the <see cref="Storage.Message"/> object,
            /// null when not available
            /// </summary>
            private MessageImportance? _importance;

            /// <summary>
            /// Contains all the <see cref="Storage.Attachment"/> and <see cref="Storage.Message"/> objects.
            /// </summary>
            private readonly List<object> _attachments = new List<object>();

            /// <summary>
            /// Contains the subject prefix of the <see cref="Storage.Message"/> object
            /// </summary>
            private string _subjectPrefix;

            /// <summary>
            /// Contains the subject of the <see cref="Storage.Message"/> object
            /// </summary>
            private string _subject;

            /// <summary>
            /// Contains the normalized subject of the <see cref="Storage.Message"/> object
            /// </summary>
            private string _subjectNormalized;

            /// <summary>
            /// Contains the text body of the <see cref="Storage.Message"/> object
            /// </summary>
            private string _bodyText;

            /// <summary>
            /// Contains the html body of the <see cref="Storage.Message"/> object
            /// </summary>
            private string _bodyHtml;

            /// <summary>
            /// Contains the rtf body of the <see cref="Storage.Message"/> object
            /// </summary>
            private string _bodyRtf;

            /// <summary>
            /// Contains the the Windows LCID of the end user who created this <see cref = "Storage.Message" />
            /// </summary>
            private RegionInfo _messageLocalId;

            /// <summary>
            /// Contains the <see cref="Storage.Flag"/> object
            /// </summary>
            private Flag _flag;

            /// <summary>
            /// Contains the <see cref="Storage.Task"/> object
            /// </summary>
            private Task _task;

            /// <summary>
            /// Contains the <see cref="Storage.Appointment"/> object
            /// </summary>
            private Appointment _appointment;

            /// <summary>
            /// Contains the <see cref="Storage.Contact"/> object
            /// </summary>
            private Contact _contact;

            /// <summary>
            /// Contains the <see cref="Storage.ReceivedBy"/> object
            /// </summary>
            private ReceivedBy _receivedBy;

            /// <summary>
            /// The conversation index
            /// </summary>
            private string _conversationIndex;

            /// <summary>
            /// The conversation topic
            /// </summary>
            private string _conversationTopic;

            /// <summary>
            /// The message size
            /// </summary>
            private int? _messageSize;

            /// <summary>
            /// The transport message headers
            /// </summary>
            private string _transportMessageHeaders;

            #endregion

            #region Properties
            /// <summary>
            /// Returns the ID of the message when the MSG file has been sent across the internet 
            /// (as specified in [RFC2822]). Null when not available
            /// </summary>
            public string Id => GetMapiPropertyString(MapiTags.PR_INTERNET_MESSAGE_ID);

            #region Type
            /// <summary>
            /// Gives the <see cref="MessageType">type</see> of this message object
            /// </summary>
            public MessageType Type
            {
                get
                {
                    if (_type != MessageType.Unknown)
                        return _type;

                    var type = GetMapiPropertyString(MapiTags.PR_MESSAGE_CLASS);

                    if (type == null)
                        return MessageType.Unknown;

                    switch (type.ToUpperInvariant())
                    {
                        case "IPM.NOTE":
                            _type = MessageType.Email;
                            break;

                        case "IPM.NOTE.RULES.OOFTEMPLATE.MICROSOFT":
                            _type = MessageType.EmailTemplateMicrosoft;
                            break;

                        case "IPM.NOTE.MOBILE.SMS":
                            _type = MessageType.EmailSms;
                            break;

                        case "REPORT.IPM.NOTE.NDR":
                            _type = MessageType.EmailNonDeliveryReport;
                            break;

                        case "REPORT.IPM.NOTE.DR":
                            _type = MessageType.EmailDeliveryReport;
                            break;

                        case "REPORT.IPM.NOTE.DELAYED":
                            _type = MessageType.EmailDelayedDeliveryReport;
                            break;

                        case "REPORT.IPM.NOTE.IPNRN":
                            _type = MessageType.EmailReadReceipt;
                            break;

                        case "REPORT.IPM.NOTE.IPNNRN":
                            _type = MessageType.EmailNonReadReceipt;
                            break;

                        case "IPM.NOTE.SMIME":
                            _type = MessageType.EmailEncryptedAndMaybeSigned;
                            break;

                        case "REPORT.IPM.NOTE.SMIME.NDR":
                            _type = MessageType.EmailEncryptedAndMaybeSignedNonDelivery;
                            break;

                        case "REPORT.IPM.NOTE.SMIME.DR":
                            _type = MessageType.EmailEncryptedAndMaybeSignedDelivery;
                            break;

                        case "IPM.NOTE.SMIME.MULTIPARTSIGNED":
                            _type = MessageType.EmailClearSigned;
                            break;

                        case "IPM.NOTE.RECEIPT.SMIME.MULTIPARTSIGNED":
                            _type = MessageType.EmailClearSigned;
                            break;

                        case "REPORT.IPM.NOTE.SMIME.MULTIPARTSIGNED.NDR":
                            _type = MessageType.EmailClearSignedNonDelivery;
                            break;

                        case "REPORT.IPM.NOTE.SMIME.MULTIPARTSIGNED.DR":
                            _type = MessageType.EmailClearSignedDelivery;
                            break;

                        case "IPM.NOTE.BMA.STUB":
                            _type = MessageType.EmailBmaStub;
                            break;

                        case "IPM.APPOINTMENT":
                            _type = MessageType.Appointment;
                            break;

                        case "IPM.SCHEDULE.MEETING":
                            _type = MessageType.AppointmentSchedule;
                            break;

                        case "IPM.NOTIFICATION.MEETING":
                            _type = MessageType.AppointmentNotification;
                            break;

                        case "IPM.SCHEDULE.MEETING.REQUEST":
                            _type = MessageType.AppointmentRequest;
                            break;

                        case "IPM.SCHEDULE.MEETING.REQUEST.NDR":
                            _type = MessageType.AppointmentRequestNonDelivery;
                            break;

                        case "IPM.SCHEDULE.MEETING.CANCELED":
                            _type = MessageType.AppointmentResponseCanceled;
                            break;

                        case "IPM.SCHEDULE.MEETING.CANCELED.NDR":
                            _type = MessageType.AppointmentResponseCanceledNonDelivery;
                            break;
                        
                        case "IPM.SCHEDULE.MEETING.RESPONSE":
                            _type = MessageType.AppointmentResponse;
                            break;

                        case "IPM.SCHEDULE.MEETING.RESP.POS":
                            _type = MessageType.AppointmentResponsePositive;
                            break;

                        case "IPM.SCHEDULE.MEETING.RESP.POS.NDR":
                            _type = MessageType.AppointmentResponsePositiveNonDelivery;
                            break;

                        case "IPM.SCHEDULE.MEETING.RESP.NEG":
                            _type = MessageType.AppointmentResponseNegative;
                            break;

                        case "IPM.SCHEDULE.MEETING.RESP.NEG.NDR":
                            _type = MessageType.AppointmentResponseNegativeNonDelivery;
                            break;

                        case "IPM.SCHEDULE.MEETING.RESP.TENT":
                            _type = MessageType.AppointmentResponseTentative;
                            break;

                        case "IPM.SCHEDULE.MEETING.RESP.TENT.NDR":
                            _type = MessageType.AppointmentResponseTentativeNonDelivery;
                            break;

                        case "IPM.CONTACT":
                            _type = MessageType.Contact;
                            break;

                        case "IPM.TASK":
                            _type = MessageType.Task;
                            break;

                        case "IPM.TASKREQUEST.ACCEPT":
                            _type = MessageType.TaskRequestAccept;
                            break;

                        case "IPM.TASKREQUEST.DECLINE":
                            _type = MessageType.TaskRequestDecline;
                            break;

                        case "IPM.TASKREQUEST.UPDATE":
                            _type = MessageType.TaskRequestUpdate;
                            break;

                        case "IPM.STICKYNOTE":
                            _type = MessageType.StickyNote;
                            break;

                        case "IPM.NOTE.CUSTOM.CISCO.UNITY.VOICE":
                            _type = MessageType.CiscoUnityVoiceMessage;
                            break;

                        case "IPM.NOTE.RIGHTFAX.ADV":
                            _type = MessageType.RightFaxAdv;
                            break;
                        
                        case "IPM.NOTE.MICROSOFT.MISSED":
                            _type = MessageType.SkypeForBusinessMissedMessage;
                            break;

                        case "IPM.NOTE.MICROSOFT.CONVERSATION":
                            _type = MessageType.SkypeForBusinessConversation;
                            break;
                    }

                    return _type;
                }
            }
            #endregion

            /// <summary>
            /// Returns the filename of the message object. For message objects Outlook uses the subject. It strips
            /// invalid filename characters. When there is no filename the name from <see cref="LanguageConsts.NameLessFileName"/>
            /// will be used
            /// </summary>
            public string FileName
            {
                get
                {
                    if (_fileName != null)
                        return _fileName;

                    _fileName = GetMapiPropertyString(MapiTags.PR_SUBJECT);

                    if (string.IsNullOrEmpty(_fileName))
                        _fileName = LanguageConsts.NameLessFileName;

                    _fileName = FileManager.RemoveInvalidFileNameChars(_fileName) + ".msg";
                    return _fileName;
                }
            }

            /// <summary>
            /// Returns the date and time when the message was created or null
            /// when not available
            /// </summary>
            public DateTime? CreationTime => _creationTime ?? (_creationTime = GetMapiPropertyDateTime(MapiTags.PR_CREATION_TIME));

            /// <summary>
            /// Returns the name of the last user (or creator) that has changed the Message object or
            /// null when not available
            /// </summary>
            public string LastModifierName => _lastModifierName ??
                                                (_lastModifierName = GetMapiPropertyString(MapiTags.PR_LAST_MODIFIER_NAME_W));

            /// <summary>
            /// Returns the date and time when the message was last modified or null
            /// when not available
            /// </summary>
            public DateTime? LastModificationTime => _lastModificationTime ??
                                                     (_lastModificationTime =
                                                         GetMapiPropertyDateTime(MapiTags.PR_LAST_MODIFICATION_TIME));

            /// <summary>
            /// Returns the raw Transport Message Headers
            /// </summary>
            public string TransportMessageHeaders => _transportMessageHeaders;
            /// <summary>
            /// Returns the sender of the Message
            /// </summary>
            // ReSharper disable once CSharpWarnings::CS0109
            public new Sender Sender { get; private set; }

            /// <summary>
            /// Returns the representing sender of the Message, null when not available
            /// </summary>
            // ReSharper disable once CSharpWarnings::CS0109
            public new SenderRepresenting SenderRepresenting { get; private set; }
            
            /// <summary>
            /// Returns the list of recipients in the message object
            /// </summary>
            public List<Recipient> Recipients => _recipients;

            /// <summary>
            /// Returns an URL to the help page of an mailing list when this message is part of a mailing
            /// or null when not available
            /// </summary>
            public string MailingListHelp
            {
                get
                {
                    if (!string.IsNullOrEmpty(_mailingListHelp))
                        return _mailingListHelp;

                    _mailingListHelp = GetMapiPropertyString(MapiTags.PR_LIST_HELP);

                    if (_mailingListHelp == null) return null;

                    if (_mailingListHelp.StartsWith("<"))
                        _mailingListHelp = _mailingListHelp.Substring(1);

                    if (_mailingListHelp.EndsWith(">"))
                        _mailingListHelp = _mailingListHelp.Substring(0, _mailingListHelp.Length - 1);

                    return _mailingListHelp;
                }
            }

            /// <summary>
            /// Returns an URL to the subscribe page of an mailing list when this message is part of a mailing
            /// or null when not available
            /// </summary>
            public string MailingListSubscribe
            {
                get
                {
                    if (!string.IsNullOrEmpty(_mailingListSubscribe))
                        return _mailingListSubscribe;

                    _mailingListSubscribe = GetMapiPropertyString(MapiTags.PR_LIST_SUBSCRIBE);

                    if (_mailingListSubscribe == null) return null;

                    if (_mailingListSubscribe.StartsWith("<"))
                        _mailingListSubscribe = _mailingListSubscribe.Substring(1);

                    if (_mailingListSubscribe.EndsWith(">"))
                        _mailingListSubscribe = _mailingListSubscribe.Substring(0, _mailingListSubscribe.Length - 1);

                    return _mailingListSubscribe;
                }
            }

            /// <summary>
            /// Returns an URL to the unsubscribe page of an mailing list when this message is part of a mailing
            /// </summary>
            public string MailingListUnsubscribe
            {
                get
                {
                    if (!string.IsNullOrEmpty(_mailingListUnsubscribe))
                        return _mailingListUnsubscribe;

                    _mailingListUnsubscribe = GetMapiPropertyString(MapiTags.PR_LIST_UNSUBSCRIBE);

                    if (_mailingListUnsubscribe == null) return null;

                    if (_mailingListUnsubscribe.StartsWith("<"))
                        _mailingListUnsubscribe = _mailingListUnsubscribe.Substring(1);

                    if (_mailingListUnsubscribe.EndsWith(">"))
                        _mailingListUnsubscribe = _mailingListUnsubscribe.Substring(0, _mailingListUnsubscribe.Length - 1);

                    return _mailingListUnsubscribe;
                }
            }

            /// <summary>
            /// Returns the date/time in UTC format when the message object has been sent, null when not available
            /// </summary>
            public DateTime? SentOn
            {
                get
                {
                    if (_sentOn != null)
                        return _sentOn;

                    _sentOn = GetMapiPropertyDateTime(MapiTags.PR_CLIENT_SUBMIT_TIME) ??
                                    GetMapiPropertyDateTime(MapiTags.PR_PROVIDER_SUBMIT_TIME);

                    if (_sentOn == null && Headers != null)
                        _sentOn = Headers.DateSent.ToLocalTime();

                    return _sentOn;
                }
            }

            /// <summary>
            /// PR_MESSAGE_DELIVERY_TIME  is the time that the message was delivered to the store and 
            /// PR_CLIENT_SUBMIT_TIME  is the time when the message was sent by the client (Outlook) to the server.
            /// Now in this case when the Outlook is offline, it refers to the local store. Therefore when an email is sent, 
            /// it gets submitted to the local store and PR_MESSAGE_DELIVERY_TIME  gets set the that time. Once the Outlook is 
            /// online at that point the message gets submitted by the client to the server and the PR_CLIENT_SUBMIT_TIME  gets stamped. 
            /// Null when not available
            /// </summary>
            public DateTime? ReceivedOn
            {
                get
                {
                    if (_receivedOn != null)
                        return _receivedOn;

                    _receivedOn = GetMapiPropertyDateTime(MapiTags.PR_MESSAGE_DELIVERY_TIME);

                    if (_receivedOn == null && Headers?.Received != null && Headers.Received.Count > 0)
                        _receivedOn = Headers.Received[0].Date.ToLocalTime();

                    return _receivedOn;
                }
            }

            /// <summary>
            /// Returns the <see cref="MessageImportance"/> of the <see cref="Storage.Message"/> object, null when not available
            /// </summary>
            public MessageImportance? Importance
            {
                get
                {
                    if (_importance != null)
                        return _importance;

                    var importance = GetMapiPropertyInt32(MapiTags.PR_IMPORTANCE);
                    if (importance == null)
                    {
                        _importance = MessageImportance.Normal;
                        return _importance;
                    }

                    switch (importance)
                    {
                        case 0:
                            _importance = MessageImportance.Low;
                            break;

                        case 1:
                            _importance = MessageImportance.Normal;
                            break;

                        case 2:
                            _importance = MessageImportance.High;
                            break;
                    }

                    return _importance;
                }
            }

            /// <summary>
            /// Returns the <see cref="MessageImportance"/> of the <see cref="Storage.Message"/> object object as text
            /// </summary>
            public string ImportanceText
            {
                get
                {
                    if (Importance == null)
                        return LanguageConsts.ImportanceNormalText;

                    switch (Importance)
                    {
                        case MessageImportance.Low:
                            return LanguageConsts.ImportanceLowText;

                        case MessageImportance.Normal:
                            return LanguageConsts.ImportanceNormalText;

                        case MessageImportance.High:
                            return LanguageConsts.ImportanceHighText;

                    }

                    return LanguageConsts.ImportanceNormalText;
                }
            }

            /// <summary>
            /// Returns a list with <see cref="Storage.Attachment"/> and/or <see cref="Storage.Message"/> 
            /// objects that are attachted to the <see cref="Storage.Message"/> object
            /// </summary>
            public List<object> Attachments => _attachments;

            /// <summary>
            /// Returns the rendering position of this <see cref="Storage.Message"/> object when it was added to another
            /// <see cref="Storage.Message"/> object and the body type was set to RTF
            /// </summary>
            public int RenderingPosition { get; }

            /// <summary>
            ///     Returns or sets the subject prefix of the E-mail
            /// </summary>
            public string SubjectPrefix
            {
                get
                {
                    if (_subjectPrefix != null)
                        return _subjectPrefix;

                    _subjectPrefix = GetMapiPropertyString(MapiTags.PR_SUBJECT_PREFIX);
                    if (string.IsNullOrEmpty(_subjectPrefix))
                        _subjectPrefix = string.Empty;

                    return _subjectPrefix;
                }
            }

            /// <summary>
            /// Returns the subject of the <see cref="Storage.Message"/> object
            /// </summary>
            public string Subject
            {
                get
                {
                    if (_subject != null)
                        return _subject;

                    _subject = GetMapiPropertyString(MapiTags.PR_SUBJECT);
                    if (string.IsNullOrEmpty(_subject))
                        _subject = string.Empty;

                    return _subject;
                }
            }

            /// <summary>
            ///     Returns the normalized subject of the E-mail
            /// </summary>
            public string SubjectNormalized
            {
                get
                {
                    if (_subjectNormalized != null)
                        return _subjectNormalized;

                    _subjectNormalized = GetMapiPropertyString(MapiTags.PR_NORMALIZED_SUBJECT);
                    if (string.IsNullOrEmpty(_subjectNormalized))
                        _subjectNormalized = string.Empty;

                    return _subjectNormalized;
                }
            }

            /// <summary>
            /// Returns the available E-mail headers. These are only filled when the message
            /// has been sent accross the internet. Returns null when there aren't any message headers
            /// </summary>
            public MessageHeader Headers { get; private set; }

            // ReSharper disable once CSharpWarnings::CS0109
            /// <summary>
            /// Returns a <see cref="Flag"/> object when a flag has been set on the <see cref="Storage.Message"/>.
            /// Returns null when not available.
            /// </summary>
            public new Flag Flag
            {
                get
                {
                    if (_flag != null)
                        return _flag;

                    var flag = new Flag(this);

                    if (flag.Request != null)
                        _flag = flag;

                    return _flag;
                }
            }

            // ReSharper disable once CSharpWarnings::CS0109
            /// <summary>
            /// Returns an <see cref="Appointment"/> object when the <see cref="MessageType"/> is a <see cref="MessageType.Appointment"/>.
            /// Returns null when not available.
            /// </summary>
            public new Appointment Appointment
            {
                get
                {
                    if (_appointment != null)
                        return _appointment;

                    switch (Type)
                    {
                        case MessageType.AppointmentRequest:
                        case MessageType.Appointment:
                        case MessageType.AppointmentResponse:
                        case MessageType.AppointmentResponsePositive:
                        case MessageType.AppointmentResponseNegative:
                            break;

                        default:
                            return null;
                    }

                    _appointment = new Appointment(this);
                    return _appointment;
                }
            }

            // ReSharper disable once CSharpWarnings::CS0109
            /// <summary>
            /// Returns a <see cref="Task"/> object. This property is only available when: <br/>
            /// - The <see cref="Storage.Message.Type"/> is an <see cref="MessageType.Email"/> and the <see cref="Flag"/> object is not null<br/>
            /// - The <see cref="Storage.Message.Type"/> is an <see cref="MessageType.Task"/> or <see cref="MessageType.TaskRequestAccept"/> <br/>
            /// </summary>
            public new Task Task
            {
                get
                {
                    if (_task != null)
                        return _task;

                    switch (_type)
                    {
                        case MessageType.Email:
                            if (Flag == null)
                                return null;
                            break;

                        case MessageType.Task:
                        case MessageType.TaskRequestAccept:
                            break;

                        default:
                            return null;
                    }

                    _task = new Task(this);
                    return _task;
                }
            }

            // ReSharper disable once CSharpWarnings::CS0109
            /// <summary>
            /// Returns a <see cref="Storage.Contact"/> object when the <see cref="MessageType"/> is a <see cref="MessageType.Contact"/>.
            /// Returns null when not available.
            /// </summary>
            public new Contact Contact
            {
                get
                {
                    if (_contact != null)
                        return _contact;

                    switch (Type)
                    {
                        case MessageType.Contact:
                            break;

                        default:
                            return null;
                    }

                    _contact = new Contact(this);
                    return _contact;
                }
            }

            /// <summary>
            /// Returns the categories that are placed in the Outlook message.
            /// Only supported for outlook messages from Outlook 2007 or higher
            /// </summary>
            public ReadOnlyCollection<string> Categories => GetMapiPropertyStringList(MapiTags.Keywords);

            /// <summary>
            /// Returns the body of the Outlook message in plain text format.
            /// </summary>
            /// <value> The body of the Outlook message in plain text format. </value>
            public string BodyText
            {
                get
                {
                    if (_bodyText != null)
                        return _bodyText;

                    _bodyText = GetMapiPropertyString(MapiTags.PR_BODY);
                    return _bodyText;
                }
            }

            /// <summary>
            /// Returns the body of the Outlook message in RTF format.
            /// </summary>
            /// <value> The body of the Outlook message in RTF format. </value>
            public string BodyRtf
            {
                get
                {
                    if (_bodyRtf != null)
                        return _bodyRtf;

                    // Get value for the RTF compressed MAPI property
                    var rtfBytes = GetMapiPropertyBytes(MapiTags.PR_RTF_COMPRESSED);

                    // Return null if no property value exists
                    if (rtfBytes == null || rtfBytes.Length == 0)
                        return null;

                    rtfBytes = RtfDecompressor.DecompressRtf(rtfBytes);
                    _bodyRtf = MessageCodePage.GetString(rtfBytes);
                    return _bodyRtf;
                }
            }

            /// <summary>
            /// Returns the body of the Outlook message in HTML format.
            /// </summary>
            /// <value> The body of the Outlook message in HTML format. </value>
            public string BodyHtml
            {
                get
                {
                    if (_bodyHtml != null)
                        return _bodyHtml;

                    string html = null;

                    // Always try to get the HTML from the RTF in favor if the PR_HTML tag
                    var bodyRtf = BodyRtf;
                    if (bodyRtf != null)
                    {
                        var rtfDomDocument = new Rtf.DomDocument();
                        rtfDomDocument.LoadRtfText(bodyRtf);
                        if (!string.IsNullOrEmpty(rtfDomDocument.HtmlContent))
                            html = rtfDomDocument.HtmlContent.Trim('\r', '\n');
                    }

                    if (string.IsNullOrEmpty(html))
                    {
                        // Get value for the HTML MAPI property
                        var htmlObject = GetMapiProperty(MapiTags.PR_BODY_HTML);

                        switch (htmlObject)
                        {
                            case string s:
                            {
                                var bytes = Encoding.Default.GetBytes(s);
                                html = InternetCodePage.GetString(bytes);
                                break;
                            }

                            case byte[] htmlByteArray:
                                html = InternetCodePage.GetString(htmlByteArray);
                                break;
                        }
                    }

                    _bodyHtml = html;
                    return _bodyHtml;
                }
            }

            /// <summary>
            /// Returns the the <see cref="RegionInfo"/> for the Windows LCID of the end user who created this
            /// <see cref="Storage.Message"/> It will return <c>null</c> when the the Windows LCID could not be 
            /// read from the <see cref="Storage.Message"/>
            /// </summary>
            public RegionInfo MessageLocalId
            {
                get
                {
                    if (_messageLocalId != null)
                        return _messageLocalId;

                    var lcid = GetMapiPropertyInt32(MapiTags.PR_MESSAGE_LOCALE_ID);

                    if (!lcid.HasValue) return null;
                    _messageLocalId = new RegionInfo(lcid.Value);

                    return null;
                }
            }

            /// <summary>
            /// Returns true when the signature is valid when the <see cref="MessageType"/> is a <see cref="MessageType.EmailEncryptedAndMaybeSigned"/>.
            /// It will return null when the signature is invalid or the <see cref="Storage.Message"/> has another <see cref="MessageType"/>
            /// </summary>
            public bool? SignatureIsValid { get; private set; }

            /// <summary>
            /// Returns the name of the person who signed the <see cref="Storage.Message"/> when the <see cref="MessageType"/> is a 
            /// <see cref="MessageType.EmailEncryptedAndMaybeSigned"/>. It will return null when the signature is invalid or the <see cref="Storage.Message"/> 
            /// has another <see cref="MessageType"/>
            /// </summary>
            public string SignedBy { get; private set; }

            /// <summary>
            /// Returns the date and time when the <see cref="Storage.Message"/> has been signed when the <see cref="MessageType"/> is a 
            /// <see cref="MessageType.EmailEncryptedAndMaybeSigned"/>. It will return null when the signature is invalid or the <see cref="Storage.Message"/> 
            /// has another <see cref="MessageType"/>
            /// </summary>
            public DateTime? SignedOn { get; private set; }

            /// <summary>
            /// Returns the certificate that has been used to sign the <see cref="Storage.Message"/> when the <see cref="MessageType"/> is a 
            /// <see cref="MessageType.EmailEncryptedAndMaybeSigned"/>. It will return null when the signature is invalid or the <see cref="Storage.Message"/> 
            /// has another <see cref="MessageType"/>
            /// </summary>
            public X509Certificate2 SignedCertificate { get; private set; }
            
            /// <summary>
            /// Returns information about who has received this message. This information is only
            /// set when a message has been received and when the message provider stamped this 
            /// information into this message. Null when not available.
            /// </summary>
#pragma warning disable 109
            public new ReceivedBy ReceivedBy
#pragma warning restore 109
            {
                get
                {
                    if (_receivedBy != null)
                        return _receivedBy;

                    _receivedBy = new ReceivedBy(
                        GetMapiPropertyString(MapiTags.PR_RECEIVED_BY_ADDRTYPE), 
                        GetMapiPropertyString(MapiTags.PR_RECEIVED_BY_EMAIL_ADDRESS),
                        GetMapiPropertyString(MapiTags.PR_RECEIVED_BY_NAME));
                    return _receivedBy;
                }
            }

            /// <summary>
            /// Returns the index of the conversation. When not available <c>null</c> is returned
            /// </summary>
            public string ConversationIndex
            {
                get
                {
                    if (_conversationIndex != null)
                        return _conversationIndex;
                    var conversationIndexBytes= GetMapiProperty(MapiTags.PR_CONVERSATION_INDEX);
                    if(conversationIndexBytes is byte[] bytes)
                    {
                        _conversationIndex = BitConverter.ToString(bytes, 0);
                        if (!string.IsNullOrWhiteSpace(_conversationIndex) && _conversationIndex.Contains("-"))
                            _conversationIndex = _conversationIndex.Replace("-", "");

                    }

                    return _conversationIndex ?? (_conversationIndex = string.Empty);
                }
            }

            /// <summary>
            /// Returns the topic of the conversation. When not available <c>null</c> is returned
            /// </summary>
            public string ConversationTopic
            {
                get
                {
                    if (_conversationTopic != null)
                        return _conversationTopic;

                    _conversationTopic = GetMapiPropertyString(MapiTags.PR_CONVERSATION_TOPIC);
                    return _conversationTopic;
                }
            }


            /// <summary>
            /// Returns the size of the message. When not available <c>null</c> is returned
            /// </summary>
            public int? Size
            {
                get
                {
                    if (_messageSize != null)
                        return _messageSize;

                    _messageSize = GetMapiPropertyInt32(MapiTags.PR_MESSAGE_SIZE);
                    return _messageSize;
                }
            }
            #endregion

            #region Constructors
            /// <summary>
            ///   Initializes a new instance of the <see cref="Storage.Message" /> class from a msg file.
            /// </summary>
            /// <param name="msgfile">The msg file to load</param>
            /// <param name="fileAccess">FileAcces mode, default is Read</param>
            public Message(string msgfile, FileAccess fileAccess = FileAccess.Read) : base(msgfile, fileAccess) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="Storage.Message" /> class from a <see cref="Stream" /> containing an IStorage.
            /// </summary>
            /// <param name="storageStream"> The <see cref="Stream" /> containing an IStorage. </param>
            /// <param name="fileAccess">FileAcces mode, default is Read</param>
            public Message(Stream storageStream, FileAccess fileAccess = FileAccess.Read) : base(storageStream, fileAccess) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="Storage.Message" /> class on the specified <see cref="CFStorage"/>.
            /// </summary>
            /// <param name="storage"> The storage to create the <see cref="Storage.Message" /> on. </param>
            /// <param name="renderingPosition"></param>
            /// <param name="storageName">The name of the <see cref="CFStorage"/> that contains this message</param>
            internal Message(CFStorage storage, int renderingPosition, string storageName) : base(storage)
            {
                StorageName = storageName;
                _propHeaderSize = MapiTags.PropertiesStreamHeaderTop;
                RenderingPosition = renderingPosition;
            }
            #endregion

            #region GetHeaders
            /// <summary>
            /// Try's to read the E-mail transport headers. They are only there when a msg file has been
            /// sent over the internet. When a message stays inside an Exchange server there are not any headers
            /// </summary>
            private void GetHeaders()
            {
                Logger.WriteToLog("Getting transport headers");
                _transportMessageHeaders = GetMapiPropertyString(MapiTags.PR_TRANSPORT_MESSAGE_HEADERS);
                if (!string.IsNullOrEmpty(_transportMessageHeaders))
                    Headers = HeaderExtractor.GetHeaders(_transportMessageHeaders);
            }
            #endregion

            #region LoadStorage
            /// <summary>
            /// Processes sub storages on the specified storage to capture attachment and recipient data.
            /// </summary>
            /// <param name="storage"> The storage to check for attachment and recipient data. </param>
            protected override void LoadStorage(CFStorage storage)
            {
                Logger.WriteToLog("Loading storages and streams");

                base.LoadStorage(storage);

                foreach (var storageStatistic in _subStorageStatistics)
                {
                    // Run specific load method depending on sub storage name prefix
                    if (storageStatistic.Key.StartsWith(MapiTags.RecipStoragePrefix))
                    {
                        var recipient = new Recipient(new Storage(storageStatistic.Value)); 
                        _recipients.Add(recipient);
                    }
                    else if (storageStatistic.Key.StartsWith(MapiTags.AttachStoragePrefix))
                    {
                        switch (Type)
                        {
                            case MessageType.EmailClearSigned:
                                LoadClearSignedMessage(storageStatistic.Value);
                                break;

                            case MessageType.EmailEncryptedAndMaybeSigned:
                                LoadEncryptedAndPossibleSignedMessage(storageStatistic.Value);
                                break;

                            default:
                                LoadAttachmentStorage(storageStatistic.Value, storageStatistic.Key);
                                break;
                        }
                    }
                }

                GetHeaders();
                SetEmailSenderAndRepresentingSender();

                // Check if there is a named substorage and if so open it and map all the named MAPI properties
                if (_subStorageStatistics.ContainsKey(MapiTags.NameIdStorage))
                {
                    var mappingValues = new List<string>();

                    // Get all the named properties from the _streamStatistics
                    foreach (var streamStatistic in _streamStatistics)
                    {
                        var name = streamStatistic.Key;

                        if (name.StartsWith(MapiTags.SubStgVersion1))
                        {
                            // Get the property value
                            var propIdentString = name.Substring(12, 4);

                            // Convert it to a short
                            var value = ushort.Parse(propIdentString, NumberStyles.HexNumber);

                            // Check if the value is in the named property range (8000 to FFFE (Hex))
                            if (value >= 32768 && value <= 65534)
                            {
                                // If so then add it to perform mapping later on
                                if (!mappingValues.Contains(propIdentString))
                                    mappingValues.Add(propIdentString);
                            }
                        }
                    }

                    // Check if there is also a properties stream and if so get all the named MAPI properties from it
                    if (_streamStatistics.ContainsKey(MapiTags.PropertiesStream))
                    {
                        // Get the raw bytes for the property stream
                        var propBytes = GetStreamBytes(MapiTags.PropertiesStream);

                        for (var i = _propHeaderSize; i < propBytes.Length; i = i + 16)
                        {
                            // Get property identifer located in 3rd and 4th bytes as a hexdecimal string
                            var propIdent = new[] { propBytes[i + 3], propBytes[i + 2] };
                            var propIdentString = BitConverter.ToString(propIdent).Replace("-", string.Empty);

                            // Convert it to a short
                            var value = ushort.Parse(propIdentString, NumberStyles.HexNumber);

                            // Check if the value is in the named property range (8000 to FFFE (Hex))
                            if (value >= 32768 && value <= 65534)
                            {
                                // If so then add it to perform mapping later on
                                if (!mappingValues.Contains(propIdentString))
                                    mappingValues.Add(propIdentString);
                            }
                        }
                    }

                    // Check if there is something to map
                    if (mappingValues.Count <= 0) return;
                    // Get the Named Id Storage, we need this one to perform the mapping
                    var subStorage = _subStorageStatistics[MapiTags.NameIdStorage];

                    // Load the subStorage into our mapping class that does all the mapping magic
                    var mapiToOom = new MapiTagMapper(new Storage(subStorage));

                    // Get the mapped properties
                    _namedProperties = mapiToOom.GetMapping(mappingValues);
                }

                Logger.WriteToLog("Storages and streams loaded");
            }
            #endregion

            #region ProcessSignedContent
            /// <summary>
            /// Processes the signed content
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            private void ProcessSignedContent(byte[] data)
            {
                Logger.WriteToLog("Processing signed content");

                var signedCms = new SignedCms();
                signedCms.Decode(data);

                try
                {
                    //signedCms.CheckSignature(signedCms.Certificates, false);
                    foreach (var cert in signedCms.Certificates)
                        SignatureIsValid = cert.Verify();

                    SignatureIsValid = true;
                    foreach (var cryptographicAttributeObject in signedCms.SignerInfos[0].SignedAttributes)
                    {
                        if (cryptographicAttributeObject.Values[0] is Pkcs9SigningTime pkcs9SigningTime)
                        {
                            SignedOn = pkcs9SigningTime.SigningTime.ToLocalTime();
                        }
                    }

                    var certificate = signedCms.SignerInfos[0].Certificate;
                    if (certificate != null)
                    {
                        SignedCertificate = certificate;
                        SignedBy = certificate.GetNameInfo(X509NameType.SimpleName, false);
                    }
                }
                catch (CryptographicException)
                {
                    SignatureIsValid = false;
                }

                // Get the decoded attachment
                using (var memoryStream = new MemoryStream(signedCms.ContentInfo.Content))
                {
                    var eml = Mime.Message.Load(memoryStream);
                    if (eml.TextBody != null)
                        _bodyText = eml.TextBody.GetBodyAsText();

                    if (eml.HtmlBody != null)
                        _bodyHtml = eml.HtmlBody.GetBodyAsText();

                    foreach (var emlAttachment in eml.Attachments)
                        _attachments.Add(new Attachment(emlAttachment));
                }

                Logger.WriteToLog("Signed content processed");
            }
#endregion

            #region LoadEncryptedAndPossibleSignedMessage
            /// <summary>
            /// Load's and parses a signed message. The signed message should be in an attachment called smime.p7m
            /// </summary>
            /// <param name="storage"></param>
            private void LoadEncryptedAndPossibleSignedMessage(CFStorage storage)
            {
                // Create attachment from attachment storage
                var attachment = new Attachment(new Storage(storage), null);

                if (attachment.FileName.ToUpperInvariant() != "SMIME.P7M")
                    throw new MRInvalidSignedFile(
                        "The signed file is not valid, it should contain an attachment called smime.p7m but it didn't");

                ProcessSignedContent(attachment.Data);
            }
            #endregion

            #region LoadEncryptedSignedMessage
            /// <summary>
            /// Load's and parses a signed message
            /// </summary>
            /// <param name="storage"></param>
            private void LoadClearSignedMessage(CFStorage storage)
            {
                Logger.WriteToLog("Loading clear signed message");
                // Create attachment from attachment storage
                var attachment = new Attachment(new Storage(storage), null);

                // Get the decoded attachment
                using (var memoryStream = new MemoryStream(attachment.Data))
                {
                    var eml = Mime.Message.Load(memoryStream);
                    if (eml.TextBody != null)
                        _bodyText = eml.TextBody.GetBodyAsText();

                    if (eml.HtmlBody != null)
                        _bodyHtml = eml.HtmlBody.GetBodyAsText();

                    foreach (var emlAttachment in eml.Attachments)
                    {
                        if (emlAttachment.FileName.ToUpperInvariant() == "SMIME.P7S")
                            ProcessSignedContent(emlAttachment.Body);
                        else
                            _attachments.Add(new Attachment(emlAttachment));
                    }
                }

                Logger.WriteToLog("Clear signed message loaded");
            }
            #endregion

            #region LoadAttachmentStorage
            /// <summary>
            /// Loads the attachment data out of the specified storage.
            /// </summary>
            /// <param name="storage"> The attachment storage. </param>
            /// <param name="storageName">The name of the <see cref="CFStorage"/></param>
            private void LoadAttachmentStorage(CFStorage storage, string storageName)
            {
                Logger.WriteToLog("Loading attachment storage");

                // Create attachment from attachment storage
                var attachment = new Attachment(new Storage(storage), storageName);
                
                var attachMethod = attachment.GetMapiPropertyInt32(MapiTags.PR_ATTACH_METHOD);
                switch (attachMethod)
                {
                    case MapiTags.ATTACH_EMBEDDED_MSG:
                        // Create new Message and set parent and header size
                        var subStorage = attachment.GetMapiProperty(MapiTags.PR_ATTACH_DATA_BIN) as CFStorage;
                        var subMsg = new Message(subStorage, attachment.RenderingPosition, storageName)
                        {
                            _parentMessage = this,
                            _propHeaderSize = MapiTags.PropertiesStreamHeaderEmbedded
                        };
                        _attachments.Add(subMsg);
                        break;

                    default:
                        // Add attachment to attachment list
                        _attachments.Add(attachment);
                        break;
                }

                Logger.WriteToLog("Attachment storage loaded");
            }
            #endregion

            #region DeleteAttachment
            /// <summary>
            /// Removes the given <paramref name="attachment"/> from the <see cref="Storage.Message"/> object.
            /// </summary>
            /// <example>
            ///     message.DeleteAttachment(message.Attachments[0]);
            /// </example>
            /// <param name="attachment"></param>
            /// <exception cref="MRCannotRemoveAttachment">Raised when it is not possible to remove the <see cref="Storage.Attachment"/> or <see cref="Storage.Message"/> from
            /// the <see cref="Storage.Message"/></exception>
            public void DeleteAttachment(object attachment)
            {
                Logger.WriteToLog("Deleting attachment");

                if (FileAccess == FileAccess.Read)
                    throw new MRCannotRemoveAttachment("Cannot remove attachments when the file is not opened in Write or ReadWrite mode");

                foreach (var attachmentObject in _attachments)
                {
                    if (!attachmentObject.Equals(attachment)) continue;
                    string storageName;
                    if (attachmentObject is Attachment attach)
                    {
                        if (string.IsNullOrEmpty(attach.StorageName))
                            throw new MRCannotRemoveAttachment("The attachment '" + attach.FileName +
                                                               "' can not be removed, the storage name is unknown");

                        storageName = attach.StorageName;
                        attach.Dispose();
                    }
                    else
                    {
                        if (!(attachmentObject is Message msg))
                            throw new MRCannotRemoveAttachment(
                                "The attachment can not be removed, could not convert the attachment to an Attachment or Message object");

                        storageName = msg.StorageName;
                        msg.Dispose();
                    }

                    _attachments.Remove(attachment);
                    TopParent._rootStorage.Delete(storageName);
                    _attachmentDeleted = true;
                    break;
                }

                Logger.WriteToLog("Attachment deleted");
            }
            #endregion

            #region Copy
            /// <summary>
            /// Copies the given <paramref name="source"/> to the given <paramref name="destination"/>
            /// </summary>
            /// <param name="source"></param>
            /// <param name="destination"></param>
            private static void Copy(CFStorage source, CFStorage destination)
            {
                Logger.WriteToLog("Copying storage");

                source.VisitEntries(action =>
                {
                    if (action.IsStorage)
                    {
                        var destinationStorage = destination.AddStorage(action.Name);
                        destinationStorage.CLSID = action.CLSID;
                        destinationStorage.CreationDate = action.CreationDate;
                        destinationStorage.ModifyDate = action.ModifyDate;
                        Copy(action as CFStorage, destinationStorage);
                    }
                    else
                    {
                        var sourceStream = action as CFStream;
                        var destinationStream = destination.AddStream(action.Name);
                        if (sourceStream != null) destinationStream.SetData(sourceStream.GetData());
                    }

                }, false);

                Logger.WriteToLog("Storage copied");
            }
            #endregion

            #region Save
            /// <summary>
            /// Saves this <see cref="Storage.Message" /> to the specified <paramref name="fileName"/>
            /// </summary>
            /// <param name="fileName"> Name of the file. </param>
            public void Save(string fileName)
            {
                Logger.WriteToLog($"Saving message to file '{fileName}'");

                using (var saveFileStream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite))
                    Save(saveFileStream);

                Logger.WriteToLog("Message saved");
            }

            /// <summary>
            /// Saves this <see cref="Storage.Message"/> to the specified <paramref name="stream"/>
            /// </summary>
            /// <param name="stream"> The stream to save to. </param>
            public void Save(Stream stream)
            {
                Logger.WriteToLog("Saving message to stream");

                if (IsTopParent)
                {
                    if (_attachmentDeleted)
                    {
                        var compoundFile = new CompoundFile();
                        var rootStorage = compoundFile.RootStorage;
                        Copy(_rootStorage, rootStorage);
                        compoundFile.Save(stream);
                        compoundFile.Close();
                    }
                    else
                        _compoundFile.Save(stream);
                }
                else
                {
                    var compoundFile = new CompoundFile();
                    var sourceNameIdStorage = TopParent._rootStorage.GetStorage(MapiTags.NameIdStorage);
                    var rootStorage = compoundFile.RootStorage;
                    var destinationNameIdStorage = rootStorage.AddStorage(MapiTags.NameIdStorage);

                    Copy(sourceNameIdStorage, destinationNameIdStorage);
                    Copy(_rootStorage, rootStorage);

                    var propertiesStream = rootStorage.GetStream(MapiTags.PropertiesStream);
                    var sourceData = propertiesStream.GetData();
                    var destinationData = new byte[sourceData.Length + 8];
                    Buffer.BlockCopy(sourceData, 0, destinationData, 0, 24);
                    Buffer.BlockCopy(sourceData, 24, destinationData, 32, sourceData.Length - 24);
                    propertiesStream.SetData(destinationData);

                    compoundFile.Save(stream);
                    compoundFile.Close();
                }

                Logger.WriteToLog("Message saved to stream");
            }
            #endregion

            #region SetEmailSenderAndRepresentingSender
            /// <summary>
            /// Gets the <see cref="Sender"/> and <see cref="SenderRepresenting"/> from the <see cref="Storage.Message"/>
            /// object and sets the <see cref="Storage.Message.Sender"/> and <see cref="Storage.Message.SenderRepresenting"/>
            /// </summary>
            private void SetEmailSenderAndRepresentingSender()
            {
                Logger.WriteToLog("Getting sender and representing sender");
                var tempEmail = GetMapiPropertyString(MapiTags.PR_SENDER_EMAIL_ADDRESS);

                if (string.IsNullOrEmpty(tempEmail) || tempEmail.IndexOf('@') == -1)
                    tempEmail = GetMapiPropertyString(MapiTags.PR_SENDER_SMTP_ADDRESS);

                if (string.IsNullOrEmpty(tempEmail))
                    tempEmail = GetMapiPropertyString(MapiTags.InternetAccountName);

                if (string.IsNullOrEmpty(tempEmail))
                    tempEmail = GetMapiPropertyString(MapiTags.SenderSmtpAddressAlternate);

                MessageHeader headers = null;

                if (string.IsNullOrEmpty(tempEmail) || tempEmail.IndexOf("@", StringComparison.Ordinal) < 0)
                {
                    var senderAddressType = GetMapiPropertyString(MapiTags.PR_SENDER_ADDRTYPE);
                    if (senderAddressType != null && senderAddressType != "EX")
                    {
                        // Get address from email headers. The headers are not present when the addressType = "EX"
                        var header = GetStreamAsString(MapiTags.HeaderStreamName, Encoding.Unicode);
                        if (!string.IsNullOrEmpty(header))
                            headers = HeaderExtractor.GetHeaders(header);
                    }
                }

                // PR_PRIMARY_SEND_ACCT can contain the smtp address of an exchange account
                if (string.IsNullOrEmpty(tempEmail) || tempEmail.IndexOf("@", StringComparison.Ordinal) < 0)
                {
                    var testEmail = GetMapiPropertyString(MapiTags.PR_PRIMARY_SEND_ACCT);
                    if(!string.IsNullOrEmpty(testEmail) && testEmail.IndexOf("\u0001", StringComparison.Ordinal) > 0)
                    {
                        tempEmail = EmailAddress.GetValidEmailAddress(testEmail);
                    }
                }

                tempEmail = EmailAddress.RemoveSingleQuotes(tempEmail);
                var tempDisplayName = EmailAddress.RemoveSingleQuotes(GetMapiPropertyString(MapiTags.PR_SENDER_NAME));

                if (string.IsNullOrEmpty(tempEmail) && headers?.From != null)
                    tempEmail = EmailAddress.RemoveSingleQuotes(headers.From.Address);

                if (string.IsNullOrEmpty(tempDisplayName) && headers?.From != null)
                    tempDisplayName = headers.From.DisplayName;

                var email = tempEmail;
                var displayName = tempDisplayName;

                // Sometimes the E-mail address and displayname get swapped so check if they are valid
                if (!EmailAddress.IsEmailAddressValid(tempEmail) && EmailAddress.IsEmailAddressValid(tempDisplayName))
                {
                    // Swap then
                    email = tempDisplayName;
                    displayName = tempEmail;
                }
                else if (EmailAddress.IsEmailAddressValid(tempDisplayName))
                {
                    // If the displayname is an emailAddress then move it
                    email = tempDisplayName;
                    displayName = tempDisplayName;
                }

                if (string.Equals(tempEmail, tempDisplayName, StringComparison.InvariantCultureIgnoreCase))
                    displayName = string.Empty;

                // Set the representing sender if it is there
                Sender = new Sender(email, displayName);
                var representingAddressType = GetMapiPropertyString(MapiTags.PR_SENT_REPRESENTING_ADDRTYPE);
                tempEmail = GetMapiPropertyString(MapiTags.PR_SENT_REPRESENTING_EMAIL_ADDRESS);
                tempEmail = EmailAddress.RemoveSingleQuotes(tempEmail);
                tempDisplayName = EmailAddress.RemoveSingleQuotes(GetMapiPropertyString(MapiTags.PR_SENT_REPRESENTING_NAME));

                email = tempEmail;
                displayName = tempDisplayName;

                // Sometimes the E-mail address and displayname get swapped so check if they are valid
                if (!EmailAddress.IsEmailAddressValid(tempEmail) && EmailAddress.IsEmailAddressValid(tempDisplayName))
                {
                    // Swap then
                    email = tempDisplayName;
                    displayName = tempEmail;
                }
                else if (EmailAddress.IsEmailAddressValid(tempDisplayName))
                {
                    // If the displayname is an emailAddress then move it
                    email = tempDisplayName;
                    displayName = tempDisplayName;
                }

                if (string.Equals(tempEmail, tempDisplayName, StringComparison.InvariantCultureIgnoreCase))
                    displayName = string.Empty;

                // Set the representing sender
                if (!string.IsNullOrWhiteSpace(email))
                    SenderRepresenting = new SenderRepresenting(email, displayName, representingAddressType);
            }
            #endregion
            
            #region GetEmailSender
            /// <summary>
            /// Returns the E-mail sender address in RFC822 format, e.g. 
            /// "Pan, P (Peter)" &lt;Peter.Pan@neverland.com&gt;
            /// </summary>
            /// <returns></returns>
            public string GetEmailSenderRfc822Format()
            {
                var output = string.Empty;

                if (!string.IsNullOrEmpty(Sender.DisplayName))
                    output = "\"" + Sender.DisplayName + "\"";

                if (!string.IsNullOrEmpty(Sender.Email))
                {
                    if (!string.IsNullOrEmpty(output))
                        output += " ";

                    output += "<" + Sender.Email + ">";
                }

                return output;
            }

            /// <summary>
            /// Returns the E-mail sender address in a human readable format
            /// </summary>
            /// <param name="html">Set to true to return the E-mail address as an html string</param>
            /// <param name="convertToHref">Set to true to convert the E-mail addresses to a hyperlink. 
            /// Will be ignored when <paramref name="html"/> is set to false</param>
            /// <returns></returns>
            public string GetEmailSender(bool html, bool convertToHref)
            {
                Logger.WriteToLog("Getting e-mail sender");

                var output = string.Empty;

                var emailAddress = Sender.Email;
                var representingEmailAddress = string.Empty;
                var displayName = Sender.DisplayName;
                var representingDisplayName = string.Empty;
                var representingAddressType = string.Empty;

                if (SenderRepresenting != null)
                {
                    representingEmailAddress = SenderRepresenting.Email;
                    representingDisplayName = SenderRepresenting.DisplayName;
                    representingAddressType = SenderRepresenting.AddressType;
                }

                if (html)
                {
                    emailAddress = WebUtility.HtmlEncode(emailAddress);
                    displayName = WebUtility.HtmlEncode(displayName);
                    representingEmailAddress = WebUtility.HtmlEncode(representingEmailAddress);
                    representingDisplayName = WebUtility.HtmlEncode(representingDisplayName);
                }

                // If we want hyperlinks and the outputformat is html and the email address is set
                if (convertToHref && html && 
                    !string.IsNullOrEmpty(emailAddress))
                {
                    output += "<a href=\"mailto:" + emailAddress + "\">" +
                                (!string.IsNullOrEmpty(displayName)
                                    ? displayName
                                    : emailAddress) + "</a>";

                    if (!string.IsNullOrEmpty(representingEmailAddress) && 
                        !string.IsNullOrEmpty(emailAddress) &&
                        !emailAddress.Equals(representingEmailAddress, StringComparison.InvariantCultureIgnoreCase))
                    {
                        output += " " + LanguageConsts.EmailOnBehalfOf + " <a href=\"mailto:" + representingEmailAddress +
                                    "\">" +
                                    (!string.IsNullOrEmpty(representingDisplayName)
                                        ? representingDisplayName
                                        : representingEmailAddress) + "</a> ";
                    }
                }
                else
                {
                    string beginTag;
                    string endTag;
                    if (html)
                    {
                        beginTag = "&nbsp;&lt;";
                        endTag = "&gt;";
                    }
                    else
                    {
                        beginTag = " <";
                        endTag = ">";
                    }

                    if (!string.IsNullOrEmpty(displayName))
                        output += displayName;

                    if (!string.IsNullOrEmpty(emailAddress))
                        output += beginTag + emailAddress + endTag;

                    if (!string.IsNullOrEmpty(representingEmailAddress) &&
                        !string.IsNullOrEmpty(emailAddress) &&
                        !emailAddress.Equals(representingEmailAddress, StringComparison.InvariantCultureIgnoreCase))
                    {
                        output += " " + LanguageConsts.EmailOnBehalfOf + " ";

                        if (!string.IsNullOrEmpty(representingDisplayName))
                            output += representingDisplayName;

                        if (!string.IsNullOrEmpty(representingEmailAddress) && representingAddressType != "EX")
                        {
                            if (!string.IsNullOrWhiteSpace(representingDisplayName)) output += beginTag;
                            output += representingEmailAddress;
                            if (!string.IsNullOrWhiteSpace(representingDisplayName)) output += endTag;
                        }
                    }
                }
                
                return output;
            }
            #endregion

            #region GetEmailRecipients
            /// <summary>
            /// Returns all the recipient for the given <paramref name="type"/>
            /// </summary>
            /// <param name="type">The <see cref="RecipientType"/> to return</param>
            /// <returns></returns>
            private List<RecipientPlaceHolder> GetEmailRecipients(RecipientType type)
            {
                Logger.WriteToLog($"Getting recipients with type {type.ToString()}");

                var recipients = new List<RecipientPlaceHolder>();

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var recipient in Recipients)
                {
                    // First we filter for the correct recipient type
                    if (recipient.Type == type)
                        recipients.Add(new RecipientPlaceHolder(recipient.Email, recipient.DisplayName, recipient.AddressType));
                }

                if (recipients.Count == 0 && Headers != null)
                {
                    switch (type)
                    {
                        case RecipientType.To:
                            if (Headers.To != null)
                                recipients.AddRange(
                                    Headers.To.Select(
                                        to => new RecipientPlaceHolder(to.Address, to.DisplayName, string.Empty)));
                            break;

                        case RecipientType.Cc:
                            if (Headers.Cc != null)
                                recipients.AddRange(
                                    Headers.Cc.Select(
                                        cc => new RecipientPlaceHolder(cc.Address, cc.DisplayName, string.Empty)));
                            break;

                        case RecipientType.Bcc:
                            if (Headers.Bcc != null)
                                recipients.AddRange(
                                    Headers.Bcc.Select(
                                        bcc => new RecipientPlaceHolder(bcc.Address, bcc.DisplayName, string.Empty)));
                            break;
                    }
                }

                return recipients;
            }

            /// <summary>
            /// Returns the E-mail recipients in RFC822 format, e.g. 
            /// "Pan, P (Peter)" &lt;Peter.Pan@neverland.com&gt;
            /// </summary>
            /// <param name="type">Selects the Recipient type to retrieve</param>
            /// <returns></returns>
            public string GetEmailRecipientsRfc822Format(RecipientType type)
            {
                var output = string.Empty;

                var recipients = GetEmailRecipients(type);
                if (Appointment?.UnsendableRecipients != null)
                    recipients.AddRange(Appointment.UnsendableRecipients.GetEmailRecipients(type));

                foreach (var recipient in recipients)
                {
                    if (output != string.Empty)
                        output += ", ";

                    var tempOutput = string.Empty;

                    if (!string.IsNullOrEmpty(recipient.DisplayName))
                        tempOutput += "\"" + recipient.DisplayName + "\"";

                    if (!string.IsNullOrEmpty(recipient.Email))
                    {
                        if (!string.IsNullOrEmpty(tempOutput))
                            tempOutput += " ";

                        tempOutput += "<" + recipient.Email + ">";
                    }

                    output += tempOutput;
                }

                return output;
            }

            /// <summary>
            /// Returns the E-mail recipients in a human readable format
            /// </summary>
            /// <param name="type">Selects the Recipient type to retrieve</param>
            /// <param name="html">Set to true to return the E-mail address as an html string</param>
            /// <param name="convertToHref">Set to true to convert the E-mail addresses to hyperlinks. 
            /// Will be ignored when <param ref="html"/> is set to false</param>
            /// <returns></returns>
            public string GetEmailRecipients(RecipientType type,
                bool html,
                bool convertToHref)
            {
                Logger.WriteToLog($"Getting recipients with type {type.ToString()}");

                var output = string.Empty;

                var recipients = GetEmailRecipients(type);
                if (Appointment?.UnsendableRecipients != null)
                    recipients.AddRange(Appointment.UnsendableRecipients.GetEmailRecipients(type));

                foreach (var recipient in recipients)
                {
                    if (output != string.Empty)
                        output += "; ";

                    var emailAddress = recipient.Email;
                    var displayName = recipient.DisplayName;

                    if (convertToHref && html && !string.IsNullOrEmpty(emailAddress))
                        output += "<a href=\"mailto:" + emailAddress + "\">" +
                                    (!string.IsNullOrEmpty(displayName)
                                        ? displayName
                                        : emailAddress) + "</a>";

                    else
                    {
                        if (!string.IsNullOrEmpty(displayName))
                            output += displayName;

                        var beginTag = string.Empty;
                        var endTag = string.Empty;
                        if (!string.IsNullOrEmpty(displayName))
                        {
                            if (html)
                            {
                                beginTag = "&nbsp;&lt;";
                                endTag = "&gt;";
                            }
                            else
                            {
                                beginTag = " <";
                                endTag = ">";
                            }
                        }

                        if (!string.IsNullOrEmpty(emailAddress))
                            output += beginTag + emailAddress + endTag;
                    }
                }

                return output;
            }
            #endregion

            #region GetAttachmentNames
            /// <summary>
            /// Returns the attachments names as a comma seperated string
            /// </summary>
            /// <returns></returns>
            public string GetAttachmentNames()
            {
                Logger.WriteToLog("Getting attachment names");

                var result = new List<string>();

                foreach (var attachment in Attachments)
                {
                    // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                    if (attachment is Attachment)
                    {
                        var attach = (Attachment)attachment;
                        result.Add(attach.FileName);
                    }
                    // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                    else if (attachment is Message)
                    {
                        var msg = (Message)attachment;
                        result.Add(msg.FileName);
                    }
                }

                return string.Join(", ", result);
            }
            #endregion
        }
    }
}