<!-- src/components/SpotifyCallback.vue -->
<template>
  <div>
    <p>Processing Spotify callback...</p>
  </div>
</template>

<script>
import { mapActions } from "vuex";

export default {
  mounted() {
    const urlSearchParams = new URLSearchParams(window.location.search);
    const params = Object.fromEntries(urlSearchParams.entries());
    const token = params?.token;
    if (!token) {
      this.$router.push("/Login");
      return;
    }

    console.log(token);
    localStorage.setItem("token", token);

    this.$store.dispatch("setUser");

    this.$router.push("/RegistrationSpotify");
  },

  name: "SpotifyCallback",
  methods: {
    ...mapActions(["handleSpotifyCallback"]),
  },
};
</script>
