<template>
  <div class="block">
    <div class="columns is-mobile is-centered">
      <div class="column is-half">
        <div class="card">
          <div class="field">
            <div class="title"><strong>Log In</strong></div>
            <p class="control has-icons-left has-icons-right">
              <input
                v-bind:value="user.email"
                @input="user.email = $event.target.value"
                class="input"
                type="email"
                placeholder="Email"
              />
              <span class="icon is-small is-left">
                <i class="fas fa-envelope"></i>
              </span>
              <span class="icon is-small is-right">
                <i class="fas fa-check"></i>
              </span>
            </p>
          </div>
          <div class="field">
            <p class="control has-icons-left">
              <input
                v-bind:value="user.password"
                @input="user.password = $event.target.value"
                class="input"
                type="password"
                placeholder="Password"
              />
              <span class="icon is-small is-left">
                <i class="fas fa-lock"></i>
              </span>
            </p>
          </div>
          <div class="field">
            <p class="control">
              <button
                class="button is-success"
                @click="login"
              >
                <strong>Log In</strong>
              </button>
            </p>
            <div class="
              noSignUp">
              Don't have an account yet? <router-link
                class="button is-info is-inverted is-small"
                to="/Registration"
              >Sign up</router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
  
  <script>
import axios from "axios";
import RegisterDialogue from "./RegisterDialogue.vue";
import RegisterForm from "./RegisterForm.vue";

export default {
  components: {
    RegisterDialogue,
    RegisterForm,
  },
  data() {
    return {
      errorVisible: false,
      user: {
        email: "",
        password: "",
      },
    };
  },

  mounted() {
    console.log("mounted");
    localStorage.removeItem("token");
    this.$store.dispatch("logout");
  },

  methods: {
    showDialog() {
      this.dialogVisible = true;
    },
    async login() {
      const data = this.user;
      const response = await axios
        .post("https://localhost:7234/api/auth/login", data, {
          headers: { "Content-Type": "application/json", accept: "text/plain" },
        })
        .catch((e) => console.log(e));
      localStorage.setItem("token", response.data);

      this.$store.dispatch("setUser");

      this.$router.push("/MainPage");
    },
  },
};
</script>
  
  <style scoped>
.card {
  padding: 1rem;
}
</style>