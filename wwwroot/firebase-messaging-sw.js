importScripts("https://www.gstatic.com/firebasejs/10.7.2/firebase-app-compat.js");
importScripts("https://www.gstatic.com/firebasejs/10.7.2/firebase-messaging-compat.js");

// Firebase Configuration

const firebaseConfig = {
    apiKey: "AIzaSyDZOrJVLQc24tlu6e_TXCkJB_GnaRTPV6c",
    authDomain: "ecommerce-product-92f11.firebaseapp.com",
    projectId: "ecommerce-product-92f11",
    storageBucket: "ecommerce-product-92f11.firebasestorage.app",
    messagingSenderId: "249111678935",
    appId: "1:249111678935:web:811cdd36ea4e372b7dc592"
  };
  
  firebase.initializeApp(firebaseConfig);
    
  const messaging = firebase.messaging();
  
  messaging.onBackgroundMessage(function(payload) {
    console.log("Received background message ", payload);
 
    const notificationTitle = payload.notification.title;
    const notificationOptions = {
      body: payload.notification.body,
    };
 
    self.registration.showNotification(notificationTitle, notificationOptions);
  });