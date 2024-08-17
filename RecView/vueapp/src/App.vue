<template>
  <div class="app">
    <div v-if="user !== null ">
      <navbar />
    </div>
    <div class="block">
    </div>
    <router-view />
  </div>
</template>

<script>
import { mapState } from "vuex";
import LoginForm from "./components/LoginForm.vue";
import Navbar from "./components/Navbar.vue";

export default {
  computed: mapState(["user"]),
  components: {
    LoginForm,
    Navbar,
  },
  mounted() {
    setTimeout(() => {
      this.$store.dispatch("setUser");
    }, 2000);
  },
};
</script>

<style scoped>
.app {
  background: rgb(6, 92, 117);
  background: linear-gradient(
    352deg,
    rgba(6, 92, 117, 1) 7%,
    rgba(39, 62, 189, 1) 42%,
    rgba(9, 19, 158, 1) 86%
  );
}
</style>


