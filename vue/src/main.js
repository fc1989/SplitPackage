import Vue from 'vue';
import iView from 'iview';
import { router } from './router/index';
import { appRouter } from './router/router';
import store from './store';
import App from './app.vue';
import i18n from './locale';
import 'iview/dist/styles/iview.css';
import VueI18n from 'vue-i18n';
import util from './libs/util';
import AppConsts from './libs/appconst'

util.ajax.get('/AbpUserConfiguration/GetAll').then(result => {
    Vue.use(iView);
    window.abp = $.extend(true, abp, result.data.result);
    new Vue({
        i18n : i18n,
        el: '#app',
        router: router,
        store: store,
        render: h => h(App),
        data: {
            currentPageName: ''
        },
        mounted() {
            this.currentPageName = this.$route.name;
            // Display a list of open pages
            this.$store.commit('setOpenedList');
            this.$store.commit('initCachepage');

            //Filtering admin menu
            this.$store.commit('updateMenulist');
        },
        created() {
            let tagsList = [];
            appRouter.map((item) => {
                if (item.children.length <= 1) {
                    tagsList.push(item.children[0]);
                } else {
                    tagsList.push(...item.children);
                }
            });

            this.$store.commit('setTagsList', tagsList);
            this.$store.commit('session/setTenantId');
            abp.message.info = (message, title) => {
                this.$Modal.info({
                    title: title,
                    content: message
                })
            };

            abp.message.success = (message, title) => {
                this.$Modal.success({
                    title: title,
                    content: message
                })
            };

            abp.message.warn = (message, title) => {
                this.$Modal.warning({
                    title: title,
                    content: message
                })
            };

            abp.message.error = (message, title) => {
                var dfd = $.Deferred();
                this.$Modal.error({
                    title: title,
                    content: message,
                    onOk: () => {
                        dfd.resolve();
                    }
                });
                return dfd;
            };

            abp.message.confirm = (message, titleOrCallback, callback) => {
                var userOpts = {
                    content: message
                };
                if ($.isFunction(titleOrCallback)) {
                    callback = titleOrCallback;
                } else if (titleOrCallback) {
                    userOpts.title = titleOrCallback;
                };
                userOpts.onOk = callback || function () { };
                this.$Modal.confirm(userOpts);
            }

            abp.notify.success = (message, title, options) => {
                this.$Notice.success({
                    title: title,
                    desc: message
                })
            };

            abp.notify.info = (message, title, options) => {
                this.$Notice.info({
                    title: title,
                    desc: message
                })
            };

            abp.notify.warn = (message, title, options) => {
                this.$Notice.warning({
                    title: title,
                    desc: message
                })
            };

            abp.notify.error = (message, title, options) => {
                this.$Notice.error({
                    title: title,
                    desc: message
                })
            };
        }
    });
})